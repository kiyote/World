namespace Common.Worlds.Builder.Algorithms.ConvexHull;

/// <summary>
/// Calculates the convex hull of a set of points.
/// </summary>
/// <remarks>
/// Algorithm generated from https://en.wikipedia.org/wiki/Quickhull
/// </remarks>
internal sealed class QuickHullFactory : IConvexHullFactory {
	IReadOnlyList<IEdge> IConvexHullFactory.Create(
		IEnumerable<IPoint> points
	) {
		points = points.Distinct();
		if( points.Count() < 3 ) {
			throw new ArgumentException( "3 or more distinct points are required.", nameof( points ) );
		}

		// The hull will be a list of points wound clockwise around the set of points
		List<IPoint> hull = new List<IPoint>();
		// Find the extreme left and right
		(IPoint left, IPoint right) = FindExtrema( points );
		hull.Add( left );
		hull.Add( right );
		// Remove the extrema from the remaining points
		IEnumerable<IPoint> remaining = points.Where( p => p != left && p != right );
		// Find all points to the right of the bisector
		IEnumerable<IPoint> first = remaining.Where( p => !IsLeft( left, right, p ) );
		// And all the remaining points are to the left
		IEnumerable<IPoint> second = remaining.Except( first );

		// Calculate the top and bottom hull
		FindHull( first, left, right, hull );
		FindHull( second, right, left, hull );

		List<HullEdge> result = new List<HullEdge>();
		for( int i = 0; i < hull.Count - 1; i++ ) {
			result.Add(
				new HullEdge(
					hull[i],
					hull[i + 1]
				)
			);
		}
		result.Add(
			new HullEdge(
				hull[^1],
				hull[0]
			)
		);

		return result;
	}

	private static void FindHull(
		IEnumerable<IPoint> points,
		IPoint from,
		IPoint to,
		List<IPoint> hull
	) {
		if( !points.Any() ) {
			return;
		}
		// Find the point that makes the largest triangle
		IPoint farthest = FindFarthest( points, from, to );
		// Add the new point between from and to
		InsertAfter( from, farthest, hull );

		IEnumerable<IPoint> remaining = points.Where( p => !PointInTriangle( from, farthest, to, p ) );
		// Points to the right of from->farthest
		IEnumerable<IPoint> first = remaining.Where( p => !IsLeft( from, farthest, p ) );
		// Points to the right of farthest->to
		IEnumerable<IPoint> second = remaining.Where( p => !IsLeft( farthest, to, p ) );

		FindHull( first, from, farthest, hull );
		FindHull( second, farthest, to, hull );
	}

	private static void InsertAfter(
		IPoint from,
		IPoint point,
		List<IPoint> hull
	) {
		hull.Insert( hull.IndexOf( from ) + 1, point );
	}

	private static IPoint FindFarthest(
		IEnumerable<IPoint> points,
		IPoint from,
		IPoint to
	) {
		List<int> areas = points.Select( point =>
			Math.Abs(
				( from.X * ( to.Y - point.Y ) )
				+ ( to.X * ( point.Y - from.Y ) )
				+ ( point.X * ( from.Y - to.Y ) )
			)
		).ToList();

		int maxArea = areas.Max();
		return points.ElementAt( areas.IndexOf( maxArea ) );
	}

	private static (IPoint Left, IPoint Right) FindExtrema(
		IEnumerable<IPoint> points
	) {
		IPoint left = points.First();
		IPoint right = points.First();
		foreach( IPoint point in points ) {
			if( point.X < left.X ) {
				left = point;
			}

			if( point.X > right.X ) {
				right = point;
			}
		}

		return (left, right);
	}

	private static bool IsLeft(
		IPoint from,
		IPoint to,
		IPoint point
	) {
		return Sign( from, to, point ) > 0;
	}

	private static bool PointInTriangle(
		IPoint p1,
		IPoint p2,
		IPoint p3,
		IPoint point
	) {
		int d1 = Sign( p1, p2, point );
		int d2 = Sign( p2, p3, point );
		int d3 = Sign( p3, p1, point );

		bool hasNegative = ( d1 < 0 ) || ( d2 < 0 ) || ( d3 < 0 );
		bool hasPositive = ( d1 > 0 ) || ( d2 > 0 ) || ( d3 > 0 );

		return !( hasNegative && hasPositive );
	}

	private static int Sign(
		IPoint from,
		IPoint to,
		IPoint point
	) {
		return ( ( to.X - from.X ) * ( point.Y - from.Y ) )
			- ( ( to.Y - from.Y ) * ( point.X - from.X ) );

	}
}
