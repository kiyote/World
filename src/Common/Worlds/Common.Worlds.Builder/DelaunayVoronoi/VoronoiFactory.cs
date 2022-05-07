namespace Common.Worlds.Builder.DelaunayVoronoi;

// Logic ported from https://github.com/d3/d3-delaunay
internal sealed class VoronoiFactory : IVoronoiFactory {

	Voronoi IVoronoiFactory.Create(
		Delaunator delaunator,
		int width,
		int height
	) {
		return DoCreate( delaunator, width, height, false );
	}

	Voronoi IVoronoiFactory.Create(
		Delaunator delaunator,
		int width,
		int height,
		bool closeCells
	) {
		return DoCreate( delaunator, width, height, closeCells );
	}

	private static Voronoi DoCreate(
		Delaunator delaunator,
		int width,
		int height,
		bool closeCells
	) {
		Point[] points = MakePoints( delaunator );
		Point[] circumcenters = MakeCircumcenters( delaunator, points );
		List<Edge> edges = MakeEdges( delaunator, circumcenters );
		int[] inedges = MakeInedges( delaunator, points );
		Cell[] cells = MakeCells( delaunator, points, inedges, circumcenters, width, height, closeCells );
		List<int>[] neighbourMap = MakeNeighbourMap( delaunator, points, inedges );
		Dictionary<Cell, IReadOnlyList<Cell>> cellNeighbours = MakeNeighbours( points, cells, neighbourMap );

		return new Voronoi( edges, cells, cellNeighbours );
	}

	private static Point[] MakeCircumcenters(
		Delaunator delaunator,
		Point[] points
	) {
		int n = delaunator.Triangles.Count / 3;
		Point[] circumcenters = new Point[n];
		for( int i = 0; i < n; i++ ) {
			Point p1 = points[delaunator.Triangles[( i * 3 )]];
			Point p2 = points[delaunator.Triangles[( i * 3 ) + 1]];
			Point p3 = points[delaunator.Triangles[( i * 3 ) + 2]];

			Delaunator.Circumcenter( p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y, out double cx, out double cy );
			Point c = new Point( (int)cx, (int)cy );
			circumcenters[i] = c;
		}

		return circumcenters;
	}

	private static Cell[] MakeCells(
		Delaunator delaunator,
		Point[] points,
		int[] inedges,
		Point[] circumcenters,
		int width,
		int height,
		bool closeCells
	) {
		Cell[] cells = new Cell[points.Length];
		List<Point> boundary = new List<Point>();
		for( int i = 0; i < points.Length; i++ ) {

			// Find all the edges of this cell
			int e0 = inedges[i];
			if( e0 == -1 ) {
				break;
			}
			int e = e0;
			bool isOpen = false;
			do {
				Point c = circumcenters[e / 3];

				// If any point is outside the bounds then the cell will be open
				if( ( c.X < 0 ) || ( c.X >= width ) || ( c.Y < 0 ) || ( c.Y >= height ) ) {
					isOpen = true;
				}

				boundary.Add( c );

				e = Delaunator.NextHalfedge( e );
				if( delaunator.Triangles[e] != i ) {
					break;
				}
				e = delaunator.HalfEdges[e];

			} while( e != e0 && e != -1 );

			//TODO: Eventually figure out a way to return these in counterclockwise order
			if( closeCells ) {
				List<Point> boundaryPoints = boundary.Append( boundary.ElementAt( 0 ) ).ToList();
				cells[i] =
					new Cell(
						points[i],
						boundaryPoints,
						isOpen || boundaryPoints.Count < 4
					);
			} else {
				List<Point> boundaryPoints = boundary.ToList();
				cells[i] =
					new Cell(
						points[i],
						boundaryPoints,
						isOpen || boundaryPoints.Count < 3
					);
			}
			boundary.Clear();
		}

		return cells;
	}

	private static int[] MakeInedges(
		Delaunator delaunator,
		Point[] points
	) {
		int[] inedges = new int[points.Length];
		Array.Fill( inedges, -1 );
		int n = delaunator.HalfEdges.Count;
		for( int e = 0; e < n; e++ ) {
			int p = delaunator.Triangles[Delaunator.NextHalfedge( e )];
			if( delaunator.HalfEdges[e] == -1 || inedges[p] == -1 ) {
				inedges[p] = e;
			}
		}

		return inedges;
	}

	private static List<int>[] MakeNeighbourMap(
		Delaunator delaunator,
		Point[] points,
		int[] inedges
	) {
		int[] hullIndex = new int[points.Length];
		Array.Fill( hullIndex, -1 );
		for( int i = 0; i < delaunator.Hull.Count; i++ ) {
			hullIndex[delaunator.Hull[i]] = i;
		}

		List<int>[] neighbourMap = new List<int>[points.Length];
		List<int> neighbours = new List<int>();
		for( int i = 0; i < points.Length; i++ ) {
			int e0 = inedges[i];
			if( e0 == -1 ) {
				throw new InvalidOperationException();
			}
			int e = e0;
			int p0;
			do {
				p0 = delaunator.Triangles[e];
				neighbours.Add( p0 );
				e = Delaunator.NextHalfedge( e );
				if( delaunator.Triangles[e] != i ) {
					throw new InvalidOperationException();
				}
				e = delaunator.HalfEdges[e];
				if( e == -1 ) {
					int p = delaunator.Hull[( hullIndex[i] + 1 ) % delaunator.Hull.Count];
					if( p != p0 ) {
						neighbours.Add( p );
					}
					break;
				}

			} while( e != e0 );
			neighbourMap[i] = neighbours.ToList();
			neighbours.Clear();
		}

		return neighbourMap;
	}

	private static Dictionary<Cell, IReadOnlyList<Cell>> MakeNeighbours(
		Point[] points,
		Cell[] cells,
		List<int>[] neighbourMap
	) {
		Dictionary<Cell, IReadOnlyList<Cell>> cellNeighbours = new Dictionary<Cell, IReadOnlyList<Cell>>();
		List<Cell> neighbourList = new List<Cell>( 10 );
		for( int i = 0; i < points.Length; i++ ) {
			Cell ci = cells[i];

			for( int j = 0; j < neighbourMap[i].Count; j++ ) {
				Cell cj = cells[neighbourMap[i][j]];
				neighbourList.Add( cj );
			}

			cellNeighbours[ci] = neighbourList.ToList(); // Force a copy to be made
			neighbourList.Clear();
		}

		return cellNeighbours;
	}

	private static Point[] MakePoints(
		Delaunator delaunator
	) {

		int n = delaunator.Coords.Count / 2;
		Point[] points = new Point[n];
		for( int i = 0; i < n; i++ ) {
			points[i] =
				new Point(
					(int)delaunator.Coords[i * 2],
					(int)delaunator.Coords[( i * 2 ) + 1]
				);
		}
		return points;
	}

	private static List<Edge> MakeEdges(
		Delaunator delaunator,
		Point[] circumcenters
	) {
		List<Edge> edges = new List<Edge>();
		for( int i = 0; i < delaunator.Triangles.Count; i++ ) {
			if( i < delaunator.HalfEdges[i] ) {
				int t1 = i / 3;
				Point c1 = circumcenters[t1];

				int t2 = delaunator.HalfEdges[i] / 3;
				Point c2 = circumcenters[t2];

				edges.Add( new Edge( c1, c2 ) );
			}
		}

		return edges.Distinct().ToList();
	}
}
