namespace Common.Worlds.Builder.Algorithms.DelaunayVoronoi;

internal sealed class HullFactory : IHullFactory {

	private const float PLANE_DISTANCE_TOLERANCE = 1e-7f;
	private const int CONNECTOR_TABLE_SIZE = 2017;
	private const int Dimension = 3;

	Hull IHullFactory.Create(
		IList<Vertex> input
	) {
		var unprocessedFaces = new SimplexList();
		var singularVertices = new HashSet<Vertex>();
		var affectedFaceBuffer = new List<SimplexWrap>();
		var convexSimplexes = new List<SimplexWrap>();
		var traverseStack = new Stack<SimplexWrap>();
		var updateBuffer = new SimplexWrap[Dimension];
		var coneFaceBuffer = new List<DeferredSimplex>();
		int[] updateIndices = new int[Dimension];
		var connectorTable = new Dictionary<long, ConnectorList>();
		var beyondBuffer = new VertexArray();

		var hullSimplexes = new List<Simplex>();
		var hullVertices = new List<Vertex>();
		float[] centroid = new float[Dimension];

		List<Vertex> inputVertices = CreateInputVertices( input );
		InitConvexHull( unprocessedFaces, convexSimplexes, inputVertices, hullVertices, ref centroid );

		// Expand the convex hull and faces.
		while( unprocessedFaces.First is not null ) {
			SimplexWrap currentFace = unprocessedFaces.First;
			Vertex? currentVertex = currentFace.FurthestVertex;

			if( currentVertex is null ) {
				throw new InvalidOperationException();
			}

			UpdateCenter( currentVertex, hullVertices, ref centroid );

			// The affected faces get tagged
			TagAffectedFaces( currentFace, currentVertex, affectedFaceBuffer, traverseStack );

			// Create the cone from the currentVertex and the affected faces horizon.
			if(
				!singularVertices.Contains( currentVertex )
				&& CreateCone( currentVertex, affectedFaceBuffer, updateBuffer, coneFaceBuffer, updateIndices, centroid )
			) {
				CommitCone( currentVertex, unprocessedFaces, affectedFaceBuffer, convexSimplexes, coneFaceBuffer, connectorTable, ref beyondBuffer, hullVertices );
			} else {
				HandleSingular( currentVertex, unprocessedFaces, singularVertices, affectedFaceBuffer, convexSimplexes, hullVertices, ref centroid );
			}

			// Need to reset the tags
			int count = affectedFaceBuffer.Count;
			for( int i = 0; i < count; i++ ) {
				affectedFaceBuffer[i].Tag = 0;
			}
		}

		for( int i = 0; i < convexSimplexes.Count; i++ ) {
			SimplexWrap wrap = convexSimplexes[i];
			wrap.Tag = i;

			hullSimplexes.Add( new Simplex( Dimension ) );
		}

		for( int i = 0; i < convexSimplexes.Count; i++ ) {

			SimplexWrap wrap = convexSimplexes[i];
			Simplex simplex = hullSimplexes[i];

			simplex.IsNormalFlipped = wrap.IsNormalFlipped;

			for( int j = 0; j < Dimension; j++ ) {
				simplex.Normal[j] = wrap.Normal[j];
				simplex.Vertices[j] = wrap.Vertices[j];

				SimplexWrap? adjacentFaces = wrap.AdjacentFaces[j];
				if( adjacentFaces is not null ) {
					simplex.Adjacent[j] = hullSimplexes[adjacentFaces.Tag];
				} else {
					simplex.Adjacent[j] = null;
				}
			}

			simplex.CalculateCentroid();
		}

		return new Hull( new Vertex( centroid[0], centroid[1] ), hullVertices, hullSimplexes );
	}

	/// <summary>
	/// Find the (dimension+1) initial points and create the simplexes.
	/// </summary>
	private static void InitConvexHull(
		SimplexList unprocessedFaces,
		List<SimplexWrap> convexSimplexes,
		List<Vertex> inputVertices,
		List<Vertex> hullVertices,
		ref float[] centroid
	) {
		List<Vertex> extremes = FindExtremes( inputVertices );
		List<Vertex> initialPoints = FindInitialPoints( extremes, inputVertices );

		int numPoints = initialPoints.Count;

		// Add the initial points to the convex hull.
		for( int i = 0; i < numPoints; i++ ) {
			Vertex currentVertex = initialPoints[i];
			// update center must be called before adding the vertex.
			UpdateCenter( currentVertex, hullVertices, ref centroid );
			hullVertices.Add( currentVertex );
			inputVertices.Remove( initialPoints[i] );

			// Because of the AklTou heuristic.
			extremes.Remove( initialPoints[i] );
		}

		// Create the initial simplexes.
		SimplexWrap[] faces = InitiateFaceDatabase( hullVertices, centroid );

		int numFaces = faces.Length;

		// Init the vertex beyond buffers.
		for( int i = 0; i < numFaces; i++ ) {
			FindBeyondVertices( faces[i], inputVertices );
			if( faces[i].VerticesBeyond.Count == 0 ) {
				convexSimplexes.Add( faces[i] ); // The face is on the hull
			} else {
				unprocessedFaces.Add( faces[i] );
			}
		}
	}

	/// <summary>
	/// Create the first faces from (dimension + 1) vertices.
	/// </summary>
	private static SimplexWrap[] InitiateFaceDatabase(
		List<Vertex> hullVertices,
		float[] centroid
	) {
		SimplexWrap[] faces = new SimplexWrap[Dimension + 1];

		for( int i = 0; i < Dimension + 1; i++ ) {
			Vertex[] vertices = hullVertices.Where( ( _, j ) => i != j ).ToArray(); // Skips the i-th vertex
			var newFace = new SimplexWrap( Dimension, new VertexArray() ) {
				Vertices = vertices
			};
			Array.Sort( vertices, new VertexIdComparer() );

			CalculateFacePlane( newFace, centroid );
			faces[i] = newFace;
		}

		// update the adjacency (check all pairs of faces)
		for( int i = 0; i < Dimension; i++ ) {
			for( int j = i + 1; j < Dimension + 1; j++ ) {
				UpdateAdjacency( faces[i], faces[j] );
			}
		}

		return faces;
	}

	/// <summary>
	/// Calculates the normal and offset of the hyper-plane given by the face's vertices.
	/// </summary>
	private static bool CalculateFacePlane(
		SimplexWrap face,
		float[] centroid
	) {
		Vertex[] vertices = face.Vertices;
		float[] normal = face.Normal;
		MathHelper.FindNormalVector( vertices, normal );

		if( float.IsNaN( normal[0] ) ) {
			return false;
		}

		float offset = 0.0f;
		float centerDistance = 0.0f;
		float[] fi = vertices[0].Position;

		for( int i = 0; i < Dimension; i++ ) {
			float n = normal[i];
			offset += n * fi[i];
			centerDistance += n * centroid[i];
		}

		face.Offset = -offset;
		centerDistance -= offset;

		if( centerDistance > 0 ) {
			for( int i = 0; i < Dimension; i++ ) {
				normal[i] = -normal[i];
			}

			face.Offset = offset;
			face.IsNormalFlipped = true;
		} else {
			face.IsNormalFlipped = false;
		}

		return true;
	}

	/// <summary>
	/// Check if 2 faces are adjacent and if so, update their AdjacentFaces array.
	/// </summary>
	private static void UpdateAdjacency(
		SimplexWrap l,
		SimplexWrap r
	) {
		Vertex[] lv = l.Vertices;
		Vertex[] rv = r.Vertices;
		int i;

		// reset marks on the 1st face
		for( i = 0; i < Dimension; i++ ) {
			lv[i].Tag = 0;
		}

		// mark all vertices on the 2nd face
		for( i = 0; i < Dimension; i++ ) {
			rv[i].Tag = 1;
		}

		// find the 1st false index
		for( i = 0; i < Dimension; i++ ) {
			if( lv[i].Tag == 0 ) {
				break;
			}
		}

		// no vertex was marked
		if( i == Dimension ) {
			return;
		}

		// check if only 1 vertex wasn't marked
		for( int j = i + 1; j < Dimension; j++ ) {
			if( lv[j].Tag == 0 ) {
				return;
			}
		}

		// if we are here, the two faces share an edge
		l.AdjacentFaces[i] = r;

		// update the adj. face on the other face - find the vertex that remains marked
		for( i = 0; i < Dimension; i++ ) {
			lv[i].Tag = 0;
		}
		for( i = 0; i < Dimension; i++ ) {
			if( rv[i].Tag == 1 ) {
				break;
			}
		}
		r.AdjacentFaces[i] = l;
	}

	/// <summary>
	/// Removes the faces "covered" by the current vertex and adds the newly created ones.
	/// </summary>
	private static bool CreateCone(
		Vertex currentVertex,
		List<SimplexWrap> affectedFaceBuffer,
		SimplexWrap[] updateBuffer,
		List<DeferredSimplex> coneFaceBuffer,
		int[] updateIndices,
		float[] centroid
	) {
		int currentVertexIndex = currentVertex.Id;
		coneFaceBuffer.Clear();

		for( int fIndex = 0; fIndex < affectedFaceBuffer.Count; fIndex++ ) {
			SimplexWrap oldFace = affectedFaceBuffer[fIndex];

			// Find the faces that need to be updated
			int updateCount = 0;
			for( int i = 0; i < Dimension; i++ ) {
				SimplexWrap? af = oldFace.AdjacentFaces[i];

				if( af is null ) {
					throw new InvalidOperationException( "(3) Adjacent Face should never be null" );
				}

				if( af.Tag == 0 ) // Tag == 0 when oldFaces does not contain af
				{
					updateBuffer[updateCount] = af;
					updateIndices[updateCount] = i;
					++updateCount;
				}
			}

			for( int i = 0; i < updateCount; i++ ) {
				SimplexWrap adjacentFace = updateBuffer[i];

				int oldFaceAdjacentIndex = 0;
				SimplexWrap?[] adjFaceAdjacency = adjacentFace.AdjacentFaces;

				for( int j = 0; j < Dimension; j++ ) {
					if( ReferenceEquals( oldFace, adjFaceAdjacency[j] ) ) {
						oldFaceAdjacentIndex = j;
						break;
					}
				}

				// Index of the face that corresponds to this adjacent face
				int forbidden = updateIndices[i];

				SimplexWrap newFace;

				int oldVertexIndex;
				Vertex[] vertices;

				newFace = new SimplexWrap( Dimension, new VertexArray() );
				vertices = newFace.Vertices;

				for( int j = 0; j < Dimension; j++ ) {
					vertices[j] = oldFace.Vertices[j];
				}

				oldVertexIndex = vertices[forbidden].Id;

				int orderedPivotIndex;

				// correct the ordering
				if( currentVertexIndex < oldVertexIndex ) {
					orderedPivotIndex = 0;
					for( int j = forbidden - 1; j >= 0; j-- ) {
						if( vertices[j].Id > currentVertexIndex ) {
							vertices[j + 1] = vertices[j];
						} else {
							orderedPivotIndex = j + 1;
							break;
						}
					}
				} else {
					orderedPivotIndex = Dimension - 1;
					for( int j = forbidden + 1; j < Dimension; j++ ) {
						if( vertices[j].Id < currentVertexIndex ) {
							vertices[j - 1] = vertices[j];
						} else {
							orderedPivotIndex = j - 1;
							break;
						}
					}
				}

				vertices[orderedPivotIndex] = currentVertex;

				if( !CalculateFacePlane( newFace, centroid ) ) {
					return false;
				}

				coneFaceBuffer.Add( MakeDeferredFace( newFace, orderedPivotIndex, adjacentFace, oldFaceAdjacentIndex, oldFace ) );
			}
		}

		return true;
	}

	/// <summary>
	/// Creates a new deferred face.
	/// </summary>
	private static DeferredSimplex MakeDeferredFace(
		SimplexWrap face,
		int faceIndex,
		SimplexWrap pivot,
		int pivotIndex,
		SimplexWrap oldFace
	) {
		DeferredSimplex ret = new DeferredSimplex( face, pivot, oldFace, faceIndex, pivotIndex );

		return ret;
	}



	/// <summary>
	/// Handles singular vertex.
	/// </summary>
	private static void HandleSingular(
		Vertex currentVertex,
		SimplexList unprocessedFaces,
		HashSet<Vertex> singularVertices,
		List<SimplexWrap> affectedFaceBuffer,
		List<SimplexWrap> convexSimplexes,
		List<Vertex> hullVertices,
		ref float[] centroid
	) {
		RollbackCenter( currentVertex, hullVertices, ref centroid );
		singularVertices.Add( currentVertex );

		// This means that all the affected faces must be on the hull and that all their "vertices beyond" are singular.
		for( int fIndex = 0; fIndex < affectedFaceBuffer.Count; fIndex++ ) {
			SimplexWrap face = affectedFaceBuffer[fIndex];
			VertexArray vb = face.VerticesBeyond;
			for( int i = 0; i < vb.Count; i++ ) {
				singularVertices.Add( vb[i] );
			}

			convexSimplexes.Add( face );
			unprocessedFaces.Remove( face );
			face.VerticesBeyond = VertexArray.Empty;
		}
	}

	/// <summary>
	/// Recalculates the centroid of the current hull.
	/// </summary>
	private static void UpdateCenter(
		Vertex currentVertex,
		List<Vertex> hullVertices,
		ref float[] centroid
	) {
		int count = hullVertices.Count + 1;

		for( int i = 0; i < Dimension; i++ ) {
			centroid[i] *= ( count - 1 );
		}

		float f = 1.0f / count;

		for( int i = 0; i < Dimension; i++ ) {
			centroid[i] = f * ( centroid[i] + currentVertex.Position[i] );
		}
	}

	/// <summary>
	/// Removes the last vertex from the center.
	/// </summary>
	private static void RollbackCenter(
		Vertex currentVertex,
		List<Vertex> hullVertices,
		ref float[] centroid
	) {
		int count = hullVertices.Count + 1;

		for( int i = 0; i < Dimension; i++ ) {
			centroid[i] *= count;
		}

		float f = 1.0f / ( count - 1 );

		for( int i = 0; i < Dimension; i++ ) {
			centroid[i] = f * ( centroid[i] - currentVertex.Position[i] );
		}
	}

	private static List<Vertex> CreateInputVertices(
		IList<Vertex> input
	) {
		List<Vertex> result = new List<Vertex>();
		int count = input.Count;
		result.AddRange( input );

		for( int i = 0; i < count; i++ ) {
			result[i].Id = i;
		}

		return result;
	}

	/// <summary>
	/// Finds the extremes in all dimensions.
	/// </summary>
	private static List<Vertex> FindExtremes(
		List<Vertex> inputVertices
	) {
		List<Vertex> extremes = new List<Vertex>( 2 * Dimension );

		int vCount = inputVertices.Count;
		for( int i = 0; i < Dimension; i++ ) {
			float min = float.MaxValue, max = float.MinValue;
			int minInd = 0, maxInd = 0;

			for( int j = 0; j < vCount; j++ ) {
				float v = inputVertices[j].Position[i];

				if( v < min ) {
					min = v;
					minInd = j;
				}
				if( v > max ) {
					max = v;
					maxInd = j;
				}
			}

			if( minInd != maxInd ) {
				extremes.Add( inputVertices[minInd] );
				extremes.Add( inputVertices[maxInd] );
			} else {
				extremes.Add( inputVertices[minInd] );
			}
		}

		return extremes;
	}

	/// <summary>
	/// Computes the sum of square distances to the initial points.
	/// </summary>
	private static float GetSquaredDistanceSum(
		Vertex pivot,
		List<Vertex> initialPoints
	) {
		int initPtsNum = initialPoints.Count;
		float sum = 0.0f;

		for( int i = 0; i < initPtsNum; i++ ) {
			Vertex initPt = initialPoints[i];

			for( int j = 0; j < Dimension; j++ ) {
				float t = ( initPt.Position[j] - pivot.Position[j] );
				sum += t * t;
			}
		}

		return sum;
	}

	/// <summary>
	/// Finds (dimension + 1) initial points.
	/// </summary>
	private static List<Vertex> FindInitialPoints(
		List<Vertex> extremes,
		List<Vertex> inputVertices
	) {
		List<Vertex> initialPoints = new List<Vertex>();

		Vertex? first = null;
		Vertex? second = null;
		float maxDist = 0.0f;
		float[] temp = new float[Dimension];

		for( int i = 0; i < extremes.Count - 1; i++ ) {
			Vertex a = extremes[i];
			for( int j = i + 1; j < extremes.Count; j++ ) {
				Vertex b = extremes[j];

				MathHelper.SubtractFast( a.Position, b.Position, temp );

				float dist = MathHelper.LengthSquared( temp );

				if( dist > maxDist ) {
					first = a;
					second = b;
					maxDist = dist;
				}
			}
		}

		if(
			first is null
			|| second is null
		) {
			throw new InvalidOperationException();
		}
		initialPoints.Add( first );
		initialPoints.Add( second );

		for( int i = 2; i <= Dimension; i++ ) {
			float maximum = 0.000001f;
			Vertex? maxPoint = default;

			for( int j = 0; j < extremes.Count; j++ ) {
				Vertex extreme = extremes[j];
				if( initialPoints.Contains( extreme ) ) {
					continue;
				}

				float val = GetSquaredDistanceSum( extreme, initialPoints );

				if( val > maximum ) {
					maximum = val;
					maxPoint = extreme;
				}
			}

			if( maxPoint is not null ) {
				initialPoints.Add( maxPoint );
			} else {
				int vCount = inputVertices.Count;
				for( int j = 0; j < vCount; j++ ) {
					Vertex point = inputVertices[j];
					if( initialPoints.Contains( point ) ) {
						continue;
					}

					float val = GetSquaredDistanceSum( point, initialPoints );

					if( val > maximum ) {
						maximum = val;
						maxPoint = point;
					}
				}

				if( maxPoint is not null ) {
					initialPoints.Add( maxPoint );
				} else {
					throw new InvalidOperationException( "Singular input data error" );
				}
			}
		}

		return initialPoints;
	}

	/// <summary>
	/// Recursively traverse all the relevant faces.
	/// </summary>
	private static void TraverseAffectedFaces(
		SimplexWrap currentFace,
		Vertex currentVertex,
		List<SimplexWrap> affectedFaceBuffer,
		Stack<SimplexWrap> traverseStack
	) {

		traverseStack.Clear();
		traverseStack.Push( currentFace );
		currentFace.Tag = 1;

		while( traverseStack.Count > 0 ) {
			SimplexWrap top = traverseStack.Pop();

			for( int i = 0; i < Dimension; i++ ) {
				SimplexWrap? adjFace = top.AdjacentFaces[i];

				if( adjFace is null ) {
					throw new InvalidOperationException( "(2) Adjacent Face should never be null" );
				}

				if(
					adjFace.Tag == 0
					&& MathHelper.GetVertexDistance( currentVertex, adjFace ) >= PLANE_DISTANCE_TOLERANCE
				) {
					affectedFaceBuffer.Add( adjFace );
					adjFace.Tag = 1;
					traverseStack.Push( adjFace );
				}
			}
		}
	}

	/// <summary>
	/// Tags all faces seen from the current vertex with 1.
	/// </summary>
	private static void TagAffectedFaces(
		SimplexWrap currentFace,
		Vertex currentVertex,
		List<SimplexWrap> affectedFaceBuffer,
		Stack<SimplexWrap> traverseStack
	) {
		affectedFaceBuffer.Clear();
		affectedFaceBuffer.Add( currentFace );
		TraverseAffectedFaces( currentFace, currentVertex, affectedFaceBuffer, traverseStack );
	}

	/// <summary>
	/// Connect faces using a connector.
	/// </summary>
	private static void ConnectFace(
		SimplexConnector connector,
		Dictionary<long, ConnectorList> connectorTable
	) {
		long index = connector.HashCode % CONNECTOR_TABLE_SIZE;
		if( !connectorTable.ContainsKey( index ) ) {
			connectorTable[index] = new ConnectorList();
		}
		ConnectorList list = connectorTable[index];

		for( SimplexConnector? current = list.First; current != null; current = current.Next ) {
			if( SimplexConnector.AreConnectable( connector, current, Dimension ) ) {
				list.Remove( current );
				SimplexConnector.Connect( current, connector );
				return;
			}
		}

		list.Add( connector );
	}

	/// <summary>
	/// Check whether the vertex v is beyond the given face. If so, add it to beyondVertices.
	/// </summary>
	private static void IsBeyond(
		SimplexWrap face,
		VertexArray beyondVertices,
		Vertex v,
		ref float maxDistance,
		ref Vertex? furthestVertex
	) {
		float distance = MathHelper.GetVertexDistance( v, face );

		if( distance >= PLANE_DISTANCE_TOLERANCE ) {
			if( distance > maxDistance ) {
				maxDistance = distance;
				furthestVertex = v;
			}
			beyondVertices.Add( v );
		}
	}

	/// <summary>
	/// Used in the "initialization" code.
	/// </summary>
	private static void FindBeyondVertices(
		SimplexWrap face,
		List<Vertex> inputVertices
	) {
		VertexArray beyondVertices = face.VerticesBeyond;

		float maxDistance = float.NegativeInfinity;
		Vertex? furthestVertex = default;

		int count = inputVertices.Count;

		for( int i = 0; i < count; i++ ) {
			IsBeyond( face, beyondVertices, inputVertices[i], ref maxDistance, ref furthestVertex );
		}

		face.FurthestVertex = furthestVertex;
	}

	/// <summary>
	/// Used by update faces.
	/// </summary>
	private static void FindBeyondVertices(
		SimplexWrap face,
		VertexArray beyond,
		VertexArray beyond1,
		Vertex currentVertex,
		ref VertexArray beyondBuffer
	) {
		VertexArray beyondVertices = beyondBuffer;

		float maxDistance = float.NegativeInfinity;
		Vertex? furthestVertex = null;
		Vertex v;

		int count = beyond1.Count;

		for( int i = 0; i < count; i++ ) {
			beyond1[i].Tag = 1;
		}

		currentVertex.Tag = 0;

		count = beyond.Count;
		for( int i = 0; i < count; i++ ) {
			v = beyond[i];
			if( ReferenceEquals( v, currentVertex ) ) {
				continue;
			}
			v.Tag = 0;
			IsBeyond( face, beyondVertices, v, ref maxDistance, ref furthestVertex );
		}

		count = beyond1.Count;
		for( int i = 0; i < count; i++ ) {
			v = beyond1[i];
			if( v.Tag == 1 ) {
				IsBeyond( face, beyondVertices, v, ref maxDistance, ref furthestVertex );
			}
		}

		face.FurthestVertex = furthestVertex;

		// Pull the old switch a roo
		VertexArray temp = face.VerticesBeyond;
		face.VerticesBeyond = beyondVertices;
		if( temp.Count > 0 ) {
			temp.Clear();
		}
		beyondBuffer = temp;
	}

	/// <summary>
	/// Commits a cone and adds a vertex to the convex hull.
	/// </summary>
	private static void CommitCone(
		Vertex currentVertex,
		SimplexList unprocessedFaces,
		List<SimplexWrap> affectedFaceBuffer,
		List<SimplexWrap> convexSimplexes,
		List<DeferredSimplex> coneFaceBuffer,
		Dictionary<long, ConnectorList> connectorTable,
		ref VertexArray beyondBuffer,
		List<Vertex> hullVertices
	) {
		hullVertices.Add( currentVertex );

		// Fill the adjacency.
		for( int i = 0; i < coneFaceBuffer.Count; i++ ) {
			DeferredSimplex face = coneFaceBuffer[i];

			SimplexWrap newFace = face.Face;
			SimplexWrap adjacentFace = face.Pivot;
			SimplexWrap oldFace = face.OldFace;
			int orderedPivotIndex = face.FaceIndex;

			newFace.AdjacentFaces[orderedPivotIndex] = adjacentFace;
			adjacentFace.AdjacentFaces[face.PivotIndex] = newFace;

			// let there be a connection.
			for( int j = 0; j < Dimension; j++ ) {
				if( j == orderedPivotIndex ) {
					continue;
				}
				SimplexConnector connector = new SimplexConnector();
				connector.Update( newFace, j, Dimension );
				ConnectFace( connector, connectorTable );
			}

			// This could slightly help...
			if( adjacentFace.VerticesBeyond.Count < oldFace.VerticesBeyond.Count ) {
				FindBeyondVertices( newFace, adjacentFace.VerticesBeyond, oldFace.VerticesBeyond, currentVertex, ref beyondBuffer );
			} else {
				FindBeyondVertices( newFace, oldFace.VerticesBeyond, adjacentFace.VerticesBeyond, currentVertex, ref beyondBuffer );
			}

			// This face will definitely lie on the hull
			if( newFace.VerticesBeyond.Count == 0 ) {
				convexSimplexes.Add( newFace );
				unprocessedFaces.Remove( newFace );
				newFace.VerticesBeyond = VertexArray.Empty;
			} else // Add the face to the list
			  {
				unprocessedFaces.Add( newFace );
			}
		}

		// Recycle the affected faces.
		for( int fIndex = 0; fIndex < affectedFaceBuffer.Count; fIndex++ ) {
			SimplexWrap face = affectedFaceBuffer[fIndex];
			unprocessedFaces.Remove( face );
			//Buffer.ObjectManager.DepositFace( face );
		}
	}

}
