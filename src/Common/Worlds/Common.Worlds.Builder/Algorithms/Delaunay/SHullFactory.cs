namespace Common.Worlds.Builder.Algorithms.Delaunay;

/// <summary>
/// Creates a Delaunay graph from a set of random input points
/// </summary>
/// <remarks>
/// Algorithm derived from http://www.s-hull.org/paper/s_hull.pdf
/// </remarks>
internal sealed class SHullFactory: IDelaunayFactory {

	IReadOnlyList<IEdge> IDelaunayFactory.Create(
		IEnumerable<IPoint> points
	) {
		points = points.Distinct();
		if (points.Count() < 3) {
			throw new ArgumentException( "3 or more distinct points are required.", nameof( points ) );
		}

		// Select a seed
		IPoint firstPoint = points.First();
		Point seed = new Point( firstPoint.X, firstPoint.Y );

		// Sort the points by distance to the seed
		SortedSet<DistancePoint> sortedPoints = new SortedSet<DistancePoint>(
			points.Skip(1).Select( p => new DistancePoint( p.X, p.Y, GetDistance( p, seed ) ) )
		);

		// Find the point closest to the seed
		Point test = sortedPoints.First();

		// Find the smallest circumcircle for every point with (seed, test)
		SortedSet<Circumcircle> circumcircles = new SortedSet<Circumcircle>(
			sortedPoints.Skip(1).Select( p => FindCircumcircle( seed, test, p ) )
		);
		Circumcircle smallest = circumcircles.First();

		List<IPoint> hull = new List<IPoint>();
		// Get the found point that we used to make this circumcircle
		Point target = smallest.Points.Except( new Point[] { test, seed } ).First();
		Triangle triangle = MakeClockwise( seed, test, target );
		hull.Add( triangle.A );
		hull.Add( triangle.B );
		hull.Add( triangle.C );

		IEnumerable<Point> remainder = sortedPoints.Except( new Point[] { test, seed, target } );
		sortedPoints = new SortedSet<DistancePoint>(
			remainder.Select( p => new DistancePoint( p.X, p.Y, GetDistance( p, smallest.Center ) ) )
		);

		/*
7: sequentially add the points s_i to the porpagating 2D convex hull that is seeded with the triangle formed from x_0, x_j, x_k .
as a new point is added the facets of the 2D-hull that are visible to it form new triangles.
8: a non-overlapping triangulation of the set of points is created.
(This is an extremely fast method for creating an non-overlapping triangualtion of a 2D point set).
9: adjacent pairs of triangles of this triangulation must be 'flipped' to create a Delaunay triangulation from the initial non-overlapping triangulation.
		*/

		return new List<IEdge>();
	}

	private static Triangle MakeClockwise(
		Point a,
		Point b,
		Point c
	) {
		float cx = ( a.X + b.X + c.Y ) / 3.0f;
		float cy = ( a.Y + b.Y + c.Y ) / 3.0f;

		float u = a.X - cx;
		float v = a.Y - cy;
		float w = b.X - a.X;
		float x = b.Y - a.Y;

		float df = (-w * v) + (x * u);
		if (df > 0.0f) {
			return new Triangle(a, c, b);
		} else {
			return new Triangle(a, b, c);
		}

	}

	private static float GetDistance(
		IPoint source,
		IPoint destination
	) {
		int dx = source.X - destination.X;
		int dy = source.Y - destination.Y;
		return ( dx * dx ) + ( dy * dy );
	}

	private static Circumcircle FindCircumcircle(
		Point a,
		Point b,
		Point c
	) {
		// Algorithm derived from https://en.wikipedia.org/wiki/Circumscribed_circle

		Point offsetB = new Point( b.X - a.X, b.Y - a.Y );
		Point offsetC = new Point( c.X - a.X, c.Y - a.Y );

		float d = 2 * ( ( offsetB.X * offsetB.Y ) - ( offsetB.Y * offsetC.X ) );
		float invD = 1.0f / d;

		// c.y * (b.x^2 + b.y^2) - b.y * (c.x^2 + c.y^2)
		float cx = (invD  * ( offsetC.Y * ( ( offsetB.X * offsetB.X ) + ( offsetB.Y * offsetB.Y ) ) )) - ( offsetB.Y * ( ( offsetC.X * offsetC.X ) + ( offsetC.Y * offsetC.Y ) ) );
		// c.y * (c.x^2 + c.y^2) - c.x * (b.x^2 + b.y^2)
		float cy = ( invD * ( offsetC.Y * ( ( offsetC.X * offsetC.X ) + ( offsetC.Y * offsetC.Y ) ) ) ) - ( offsetC.X * ( ( offsetB.X * offsetB.X ) + ( offsetB.Y * offsetB.Y ) ) );

		Point center = new Point( (int)cx + a.X, (int)cy + a.Y );

		float radius = (float)Math.Sqrt( ( cx * cx ) + ( cy * cy ) );

		return new Circumcircle( center, radius, new Point[] { a, b, c } );
	}
}
