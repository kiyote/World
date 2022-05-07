namespace Common.Worlds.Builder.DelaunayVoronoi;

internal sealed class PolygonRasterizer {

	public static void Fill(
		IReadOnlyList<Point> points,
		Action<int, int> callback
	) {
		int miny = points[0].Y;
		int maxy = miny;
		for( int i = 1; i < points.Count; i++ ) {
			miny = Math.Min( miny, points[i].Y );
			maxy = Math.Max( maxy, points[i].Y );
		}

		// Single line polygon
		if( miny == maxy ) {
			int minx = points[0].X;
			int maxx = minx;
			for( int i = 1; i < points.Count; i++ ) {
				minx = Math.Min( minx, points[i].X );
				maxx = Math.Max( maxx, points[i].X );
			}

			for( int x = minx; x < maxx; x++ ) {
				callback( x, miny );
			}
		}

		// Regular polygon
		int[] intersect = new int[points.Count];
		int previous;
		int x1;
		int x2;
		int y1;
		int y2;
		for( int y = miny; y < maxy; y++ ) {
			int intersections = 0;
			for( int i = 0; i < points.Count; i++ ) {
				previous = ( i != 0 ) ? ( i - 1 ) : ( points.Count - 1 );

				y1 = points[previous].Y;
				y2 = points[i].Y;
				if( y1 < y2 ) {
					x1 = points[previous].X;
					x2 = points[i].X;
				} else if( y1 > y2 ) {
					y2 = points[previous].Y;
					y1 = points[i].Y;
					x2 = points[previous].X;
					x1 = points[i].X;
				} else {
					continue;
				}
				if( ( ( y >= y1 ) && ( y < y2 ) ) || ( ( y == maxy ) && ( y2 == maxy ) ) ) {
					intersect[intersections++] = ( ( y - y1 ) * ( x2 - x1 ) / ( y2 - y1 ) ) + x1;
				}
			}
			int[] sorted = intersect.Take( intersections ).OrderBy( i => i ).ToArray();
			for( int i = 0; i < sorted.Length; i += 2 ) {
				int minX = Math.Min( intersect[i], intersect[i + 1] );
				int count = Math.Abs( intersect[i] - intersect[i + 1] );
				for( int x = minX; x < ( minX + count ); x++ ) {
					callback( x, y );
				}
			}
		}

		for( int i = 0; i < points.Count - 1; i++ ) {
			previous = ( i != 0 ) ? ( i - 1 ) : ( points.Count - 1 );
			int y = points[i].Y;

			if( ( miny < y ) && ( points[previous].Y == y ) && ( y < maxy ) ) {
				int minX = Math.Min( points[i].X, points[previous].X );
				int count = Math.Abs( points[i].X - points[previous].X );
				for( int x = minX; x < ( minX + count ); x++ ) {
					callback( x, y );
				}
			}
		}
	}
}
