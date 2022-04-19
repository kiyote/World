using System.Runtime.CompilerServices;

namespace Common.Worlds.Builder.Algorithms.ConvexHull;

internal sealed class ConvexHullFactory : IConvexHullFactory {

	private const float DefaultPlaneDistanceTolerance = 1e-7f;
	private const float Tolerance = float.Epsilon * 5;

	IReadOnlyList<IPoint> IConvexHullFactory.Create(
		IEnumerable<IPoint> input
	) {
		if( input.Count() < 2 ) {
			throw new ArgumentException( "Insufficient points to calculate convex hull." );
		}

		List<HullPoint> points = input.Select( p => new HullPoint( p ) ).ToList();
		if( points.Count == 2 ) {
			return points;
		};

		/* The first step is to quickly identify the three to eight vertices based on the
		 * Akl-Toussaint heuristic. */
		float maxX = float.NegativeInfinity;
		int maxXIndex = -1;
		float maxY = float.NegativeInfinity;
		int maxYIndex = -1;
		float maxSum = float.NegativeInfinity;
		int maxSumIndex = -1;
		float maxDiff = float.NegativeInfinity;
		int maxDiffIndex = -1;
		float minX = float.PositiveInfinity;
		int minXIndex = -1;
		float minY = float.PositiveInfinity;
		int minYIndex = -1;
		float minSum = float.PositiveInfinity;
		int minSumIndex = -1;
		float minDiff = float.PositiveInfinity;
		int minDiffIndex = -1;

		int numPoints = points.Count;

		// search of all points to find the extrema. What is stored here is the position (or index) within
		// points and the value
		for( int i = 0; i < numPoints; i++ ) {
			HullPoint p = points[i];
			float x = p.X;
			float y = p.Y;
			float sum = x + y;
			float diff = x - y;
			if( x < minX ) {
				minXIndex = i;
				minX = x;
			}

			if( y < minY ) {
				minYIndex = i;
				minY = y;
			}

			if( x > maxX ) {
				maxXIndex = i;
				maxX = x;
			}

			if( y > maxY ) {
				maxYIndex = i;
				maxY = y;
			}

			// so that's the Akl-Toussaint (to find extrema in x and y). here, we go a step 
			// further and check the sum and difference of x and y. instead of a initial convex
			// quadrilateral we have (potentially) a convex octagon. Because we are adding or substracting
			// there is a slight time penalty, but that seems to be made up in the next two parts where
			// having more sortedlists (with fewer elements each) is faster than fewer sortedlists (with more
			// elements). 
			if( sum < minSum ) {
				minSumIndex = i;
				minSum = sum;
			}

			if( diff < minDiff ) {
				minDiffIndex = i;
				minDiff = diff;
			}

			if( sum > maxSum ) {
				maxSumIndex = i;
				maxSum = sum;
			}

			if( diff > maxDiff ) {
				maxDiffIndex = i;
				maxDiff = diff;
			}
		}
		// what if all points are on a horizontal line? temporarily set to max and min Y to min X. This'll be fixed
		// in the function: FindIntermediatePointsForLongSkinny
		if( minY == maxY ) {
			minYIndex = maxYIndex = minXIndex;
		}
		// what if all points are on a vertical line? then do the opposite
		if( minX == maxX ) {
			minXIndex = maxXIndex = minYIndex;
		}
		//put these on a list in counter-clockwise (CCW) direction
		var extremeIndices = new List<int>( new[]
		{
				minXIndex, minSumIndex, minYIndex, maxDiffIndex,
				maxXIndex, maxSumIndex, maxYIndex, minDiffIndex
			} );
		int cvxVNum = 8; //in some cases, we need to reduce from this eight to a smaller set
						 // The next two loops handle this reduction from 8 to as few as 3.
						 // In the first loop, simply check if any indices are repeated. Thanks to the CCW order,
						 // any repeat indices are adjacent on the list. Start from the back of the loop and
						 // remove towards zero.
		for( int i = cvxVNum - 1; i >= 0; i-- ) {
			int thisExtremeIndex = extremeIndices[i];
			int nextExtremeIndex = ( i == cvxVNum - 1 ) ? extremeIndices[0] : extremeIndices[i + 1];
			float dx = Math.Abs( points[thisExtremeIndex].X - points[nextExtremeIndex].X );
			float dy = Math.Abs( points[thisExtremeIndex].Y - points[nextExtremeIndex].Y );
			if( thisExtremeIndex == nextExtremeIndex || ( dx < Tolerance && dy < Tolerance ) ) {
				cvxVNum--;
				extremeIndices.RemoveAt( i );
			}
		}
		// before we check if points are on top of one another or have some round-off error issues, these
		// indices are stored and sorted numerically for use in the second half of part 2 where we go through
		// all the points a second time. 
		int[]? indicesUsed = extremeIndices.OrderBy( x => x ).ToArray();

		// create the list that is eventually returned by the function. Initially it will have the 3 to 8 extrema
		// (as is produced in the following loop).
		var convexHullCCW = new List<HullPoint>();

		// on very rare occasions (long skinny diagonal set of points), there may only be two extrema.
		// in this case just add
		if( cvxVNum == 2 ) {
			convexHullCCW = FindIntermediatePointsForLongSkinny(
				points,
				numPoints,
				indicesUsed[0],
				indicesUsed[1],
				out List<int> newUsedIndices
			);
			if( !newUsedIndices.Any() ) {
				// looks like only two indices total! so all points are co-linear.
				return new List<HullPoint> {
					new HullPoint( points[indicesUsed[0]] ),
					new HullPoint( points[indicesUsed[1]] )
				};
			}
			newUsedIndices.Add( indicesUsed[0] );
			newUsedIndices.Add( indicesUsed[1] );
			indicesUsed = newUsedIndices.OrderBy( x => x ).ToArray();
			cvxVNum = indicesUsed.Length;
		} else {
			for( int i = cvxVNum - 1; i >= 0; i-- ) {
				// in other rare cases, often due to some roundoff error, the extrema point will produce a concavity with its
				// two neighbors. Here, we check that case. If it does make a concavity we don't use it in the initial convex
				// hull (we have captured its index and will still skip it below. it will not be searched a second time).
				// counting backwards again, we grab the previous and next point and check the "cross product" to see if the 
				// vertex in convex. if it is we add it to the returned list. 
				HullPoint currentPt = points[extremeIndices[i]];
				HullPoint prevPt = points[( i == 0 ) ? extremeIndices[cvxVNum - 1] : extremeIndices[i - 1]];
				HullPoint nextPt = points[( i == cvxVNum - 1 ) ? extremeIndices[0] : extremeIndices[i + 1]];
				if( ( ( nextPt.X - currentPt.X ) * ( prevPt.Y - currentPt.Y ) ) +
					( ( nextPt.Y - currentPt.Y ) * ( currentPt.X - prevPt.X ) ) > Tolerance ) {
					//because we are counting backwards, we need to ensure that new points are added
					// to the front of the list
					convexHullCCW.Insert( 0, currentPt ); 
									 
				} else {
					cvxVNum--;
					extremeIndices.RemoveAt( i ); //the only reason to do this is to ensure that - if the loop is to 
												  //continue - that the vectors are made to the proper new adjacent vertices
				}
			}
		}

		/* Of the 3 to 8 vertices identified in the convex hull, ... */

		//This is used to limit the number of calls to convexHullCCW[] and point.X and point.Y, which 
		//can take a significant amount of time. 
		//Initialize the point locations and vectors:
		//At minimum, the convex hull must contain two points (e.g. consider three points in a near line,
		//the third point will be added later, since it was not an extreme.)
		HullPoint p0 = convexHullCCW[0];
		float p0X = p0.X;
		float p0Y = p0.Y;
		HullPoint p1 = convexHullCCW[1];
		float p1X = p1.X;
		float p1Y = p1.Y;
		float p2X = 0,
			p2Y = 0,
			p3X = 0,
			p3Y = 0,
			p4X = 0,
			p4Y = 0,
			p5X = 0,
			p5Y = 0,
			p6X = 0,
			p6Y = 0,
			p7X = 0,
			p7Y = 0;
		float v0X = p1X - p0X;
		float v0Y = p1Y - p0Y;
		float v1X,
			v1Y,
			v2X = 0,
			v2Y = 0,
			v3X = 0,
			v3Y = 0,
			v4X = 0,
			v4Y = 0,
			v5X = 0,
			v5Y = 0,
			v6X = 0,
			v6Y = 0,
			v7X = 0,
			v7Y = 0;
		//A big if statement to make sure the convex hull wraps properly, since the number of initial cvxHull points changes
		if( cvxVNum > 2 ) {
			HullPoint p2 = convexHullCCW[2];
			p2X = p2.X;
			p2Y = p2.Y;
			v1X = p2X - p1X;
			v1Y = p2Y - p1Y;
			if( cvxVNum > 3 ) {
				HullPoint p3 = convexHullCCW[3];
				p3X = p3.X;
				p3Y = p3.Y;
				v2X = p3X - p2X;
				v2Y = p3Y - p2Y;
				if( cvxVNum > 4 ) {
					HullPoint p4 = convexHullCCW[4];
					p4X = p4.X;
					p4Y = p4.Y;
					v3X = p4X - p3X;
					v3Y = p4Y - p3Y;
					if( cvxVNum > 5 ) {
						HullPoint p5 = convexHullCCW[5];
						p5X = p5.X;
						p5Y = p5.Y;
						v4X = p5X - p4X;
						v4Y = p5Y - p4Y;
						if( cvxVNum > 6 ) {
							HullPoint p6 = convexHullCCW[6];
							p6X = p6.X;
							p6Y = p6.Y;
							v5X = p6X - p5X;
							v5Y = p6Y - p5Y;
							if( cvxVNum > 7 ) {
								HullPoint p7 = convexHullCCW[7];
								p7X = p7.X;
								p7Y = p7.Y;
								v6X = p7X - p6X;
								v6Y = p7Y - p6Y;
								//Wrap around from 7
								v7X = p0X - p7X;
								v7Y = p0Y - p7Y;
							} else //Wrap around from 6
							  {
								v6X = p0X - p6X;
								v6Y = p0Y - p6Y;
							}
						} else //Wrap around from 5
						  {
							v5X = p0X - p5X;
							v5Y = p0Y - p5Y;
						}
					} else {
						//Wrap around from 4
						v4X = p0X - p4X;
						v4Y = p0Y - p4Y;
					}
				} else {
					//Wrap around from 3
					v3X = p0X - p3X;
					v3Y = p0Y - p3Y;
				}
			} else {
				//Wrap around from 2
				v2X = p0X - p2X;
				v2Y = p0Y - p2Y;
			}
		} else {
			//Wrap around from 1
			v1X = p0X - p1X;
			v1Y = p0Y - p1Y;
		}

		/* An array of arrays of new convex hull points along the sides of the polygon created by the 3 to 8 points
		 * above. These are to be sorted arrays and they are sorted by the distances (stored in sortedDistances) from the
		 * started extrema vertex to the last. We are going to make each array really big so that we don't have to waste
		 * time extending them later. The sizes array keeps the true length. */
		var sortedPoints = new HullPoint[cvxVNum][];
		float[][] sortedDistances = new float[cvxVNum][];
		int[] sizes = new int[cvxVNum];
		for( int i = 0; i < cvxVNum; i++ ) {
			sizes[i] = 0;
			sortedPoints[i] = new HullPoint[numPoints];
			sortedDistances[i] = new float[numPoints];
		}
		int indexOfUsedIndices = 0;
		int nextUsedIndex = indicesUsed[indexOfUsedIndices++]; //Note: it increments after getting the current index
		/* Now a big loop. For each of the original vertices, check them with the 3 to 8 edges to see if they 
		 * are inside or out. If they are out, add them to the proper row of the hullCands array. */
		for( int i = 0; i < numPoints; i++ ) {
			if( indexOfUsedIndices < indicesUsed.Length && i == nextUsedIndex ) {
				//in order to avoid a contains function call, we know to only check with next usedIndex in order
				nextUsedIndex =
					indicesUsed[indexOfUsedIndices++]; //Note: it increments after getting the current index
			} else {
				HullPoint? point = points[i];
				float newPointX = point.X;
				float newPointY = point.Y;
				if( AddToListAlong( sortedPoints[0], sortedDistances[0], ref sizes[0], point, newPointX, newPointY, p0X, p0Y, v0X, v0Y, Tolerance ) ) {
					continue;
				}
				if( AddToListAlong( sortedPoints[1], sortedDistances[1], ref sizes[1], point, newPointX, newPointY, p1X, p1Y, v1X, v1Y, Tolerance ) ) {
					continue;
				}
				if( AddToListAlong( sortedPoints[2], sortedDistances[2], ref sizes[2], point, newPointX, newPointY, p2X, p2Y, v2X, v2Y, Tolerance ) ) {
					continue;
				}
				if( cvxVNum == 3 ) {
					continue;
				}
				if( AddToListAlong( sortedPoints[3], sortedDistances[3], ref sizes[3], point, newPointX, newPointY, p3X, p3Y, v3X, v3Y, Tolerance ) ) {
					continue;
				}
				if( cvxVNum == 4 ) {
					continue;
				}
				if( AddToListAlong( sortedPoints[4], sortedDistances[4], ref sizes[4], point, newPointX, newPointY, p4X, p4Y, v4X, v4Y, Tolerance ) ) {
					continue;
				}
				if( cvxVNum == 5 ) {
					continue;
				}
				if( AddToListAlong( sortedPoints[5], sortedDistances[5], ref sizes[5], point, newPointX, newPointY, p5X, p5Y, v5X, v5Y, Tolerance ) ) {
					continue;
				}
				if( cvxVNum == 6 ) {
					continue;
				}
				if( AddToListAlong( sortedPoints[6], sortedDistances[6], ref sizes[6], point, newPointX, newPointY, p6X, p6Y, v6X, v6Y, Tolerance ) ) {
					continue;
				}
				if( cvxVNum == 7 ) {
					continue;
				}
				if( AddToListAlong( sortedPoints[7], sortedDistances[7], ref sizes[7], point, newPointX, newPointY, p7X, p7Y, v7X, v7Y, Tolerance ) ) {
					continue;
				}
			}
		}

		/* Now it's time to go through our array of sorted arrays. We search backwards through
		 * the current convex hull points s.t. any additions will not confuse our for-loop indexers.
		 * This approach is linear over the zig-zag polyline defined by each sorted list. This linear approach
		 * was defined long ago by a number of authors: McCallum and Avis, Tor and Middleditch (1984), or
		 * Melkman (1985) */
		for( int j = cvxVNum - 1; j >= 0; j-- ) {
			int size = sizes[j];
			if( size == 1 ) {
				/* If there is one and only one candidate, it must be in the convex hull. Add it now. */
				convexHullCCW.Insert( j + 1, sortedPoints[j][0] );
			} else if( size > 1 ) {
				/* it seems a shame to have this list since it's nearly the same as the sorted array, but
				 * it is necessary for the removal of points. */
				var pointsAlong = new List<HullPoint>() {
					/* put the known starting point as the beginning of the list.  */
					convexHullCCW[j]
				};
				for( int k = 0; k < size; k++ ) {
					pointsAlong.Add( sortedPoints[j][k] );
				}
				/* put the ending point on the end of the list. Need to check if it wraps back around to 
				 * the first in the loop (hence the simple condition). */
				if( j == cvxVNum - 1 ) {
					pointsAlong.Add( convexHullCCW[0] );
				} else {
					pointsAlong.Add( convexHullCCW[j + 1] );
				}

				/* Now starting from second from end, work backwards looks for places where the angle 
				 * between the vertices is concave (which would produce a negative value of z). */
				int i = size;
				while( i > 0 ) {
					//var currentPoint =
					double lX = pointsAlong[i].X - pointsAlong[i - 1].X, lY = pointsAlong[i].Y - pointsAlong[i - 1].Y;
					double rX = pointsAlong[i + 1].X - pointsAlong[i].X, rY = pointsAlong[i + 1].Y - pointsAlong[i].Y;
					double zValue = (lX * rY) - (lY * rX);
					if( zValue < Tolerance || ( Math.Abs( lX ) < Tolerance && Math.Abs( lY ) < Tolerance ) ) {
						/* remove any vertices that create concave angles. */
						pointsAlong.RemoveAt( i );
						/* but don't reduce k since we need to check the previous angle again. Well, 
						 * if you're back to the end you do need to reduce k (hence the line below). */
						if( i == pointsAlong.Count - 1 ) {
							i--;
						}
					} else {
						/* if the angle is convex, then continue toward the start, k-- */
						i--;
					}
				}

				/* for each of the remaining vertices in hullCands[i-1], add them to the convexHullCCW. 
				 * Here we insert them backwards (k counts down) to simplify the insert operation (k.e.
				 * since all are inserted @ i, the previous inserts are pushed up to i+1, i+2, etc. */
				for( i = pointsAlong.Count - 2; i > 0; i-- ) {
					convexHullCCW.Insert( j + 1, pointsAlong[i] );
				}
			}
		}

		return convexHullCCW;
	}

	private static List<HullPoint> FindIntermediatePointsForLongSkinny(
		IReadOnlyList<IPoint> points,
		int numPoints,
		int usedIndex1,
		int usedIndex2,
		out List<int> newUsedIndices
	) {
		newUsedIndices = new List<int>();
		float pStartX = points[usedIndex1].X;
		float pStartY = points[usedIndex1].Y;
		float spanVectorX = points[usedIndex2].X - pStartX;
		float spanVectorY = points[usedIndex2].Y - pStartY;
		float minCross = -DefaultPlaneDistanceTolerance;
		float maxCross = DefaultPlaneDistanceTolerance;
		int minCrossIndex = -1;
		int maxCrossIndex = -1;
		for( int i = 0; i < numPoints; i++ ) {
			if( i == usedIndex1 || i == usedIndex2 ) {
				continue;
			}
			IPoint p = points[i];
			float cross = ( spanVectorX * ( p.Y - pStartY ) ) + ( spanVectorY * ( pStartX - p.X ) );
			if( cross < minCross ) {
				minCrossIndex = i;
				minCross = cross;
			}
			if( cross > maxCross ) {
				maxCrossIndex = i;
				maxCross = cross;
			}
		}

		var newCvxList = new List<HullPoint> {
			new HullPoint( points[usedIndex1] )
		};
		if( minCrossIndex != -1 ) {
			newUsedIndices.Add( minCrossIndex );
			newCvxList.Add( new HullPoint( points[minCrossIndex] ) );
		}
		newCvxList.Add( new HullPoint( points[usedIndex2] ) );
		if( maxCrossIndex != -1 ) {
			newUsedIndices.Add( maxCrossIndex );
			newCvxList.Add( new HullPoint( points[maxCrossIndex] ) );
		}
		return newCvxList;
	}


	// this function adds the new point to the sorted array. The reason it is complicated is that
	// if it errors - it is because there are two points at the same distance along. So, we then
	// check if the new point or the existing one on the list should stay. Simply keep the one that is
	// furthest from the edge vector.
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	private static bool AddToListAlong(
		HullPoint[] sortedPoints,
		float[] sortedKeys,
		ref int size,
		HullPoint newPoint,
		float newPointX,
		float newPointY,
		float basePointX,
		float basePointY,
		float edgeVectorX,
		float edgeVectorY,
		float tolerance
	) {
		float vectorToNewPointX = newPointX - basePointX;
		float vectorToNewPointY = newPointY - basePointY;
		float newDxOut = ( vectorToNewPointX * edgeVectorY ) - ( vectorToNewPointY * edgeVectorX );
		if( newDxOut <= tolerance ) {
			return false;
		}
		float newDxAlong = (edgeVectorX * vectorToNewPointX) + (edgeVectorY * vectorToNewPointY);
		int index = BinarySearch( sortedKeys, size, newDxAlong );
		if( index >= 0 ) {
			// non-negative values occur when the same key is found. In this case, we only want to keep
			// the one vertex that sticks out the farthest.
			HullPoint ptOnList = sortedPoints[index];
			float onListDxOut = ( ( ptOnList.X - basePointX ) * edgeVectorY ) - ( ( ptOnList.Y - basePointY ) * edgeVectorX );
			if( newDxOut > onListDxOut ) {
				sortedPoints[index] = new HullPoint( newPoint );
			}
		} else {
			// here a new value is found. first, invert the index to find where to put it
			index = ~index;
			// as a slight time saver, we can check the two points that will surround this new point. 
			// If it makes a concave corner then don't add it. this part is actually in the middle 
			// condition ("else if (index < size)"). We don't need to perform this check if the insertion
			// is at either at. At the beginning ("index == 0"), we still need to increment the rest of the list
			if( index == 0 ) {
				for( int i = size; i > index; i-- ) {
					sortedKeys[i] = sortedKeys[i - 1];
					sortedPoints[i] = sortedPoints[i - 1];
				}
				sortedKeys[index] = newDxAlong;
				sortedPoints[index] = new HullPoint( newPoint );
				size++;
			} else if( index < size ) {
				HullPoint prevPt = sortedPoints[index - 1];
				HullPoint nextPt = sortedPoints[index];
				float lX = newPointX - prevPt.X, lY = newPointY - prevPt.Y;
				float rX = nextPt.X - newPointX, rY = nextPt.Y - newPointY;
				float zValue = ( lX * rY ) - ( lY * rX );
				// if cross produce is negative (well, is less than some small positive number, then new point is concave) then don't add it.
				// also, don't add it if the point is nearly identical (again, within the tolerance) of the previous point.
				if( zValue < tolerance || ( Math.Abs( lX ) < tolerance && Math.Abs( lY ) < tolerance ) ) {
					return false;
				}
				//Else
				for( int i = size; i > index; i-- ) {
					sortedKeys[i] = sortedKeys[i - 1];
					sortedPoints[i] = sortedPoints[i - 1];
				}
				sortedKeys[index] = newDxAlong;
				sortedPoints[index] = new HullPoint( newPoint );
				size++;
			} else {   // if at the end, then no need to increment any other members.
				sortedKeys[index] = newDxAlong;
				sortedPoints[index] = new HullPoint( newPoint );
				size++;
			}
		}
		return true;
	}

	// This binary search is modified/simplified from Array.BinarySearch
	// (https://referencesource.microsoft.com/mscorlib/a.html#b92d187c91d4c9a9)
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	private static int BinarySearch(
		float[] array,
		int length,
		float value
	) {
		int lo = 0;
		int hi = length - 1;
		while( lo <= hi ) {
			int i = lo + ( ( hi - lo ) >> 1 );
			float c = array[i];
			if( c == value ) {
				return i;
			}
			if( c < value ) {
				lo = i + 1;
			} else {
				hi = i - 1;
			}
		}
		return ~lo;
	}
}
