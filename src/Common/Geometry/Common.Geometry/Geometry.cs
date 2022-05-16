namespace Common.Geometry;

[System.Diagnostics.CodeAnalysis.SuppressMessage( "Performance", "CA1812:An internal (assembly-level) type is never instantiated.", Justification = "This class is instantiated via DI." )]
internal sealed class Geometry : IGeometry {
	int IGeometry.LineLength(
		IPoint p1,
		IPoint p2
	) {
		return (int)Math.Sqrt( Math.Pow( p2.Y - p1.Y, 2 ) + Math.Pow( p2.X - p1.X, 2 ) );
	}

	bool IGeometry.PointInPolygon(
		IReadOnlyList<IPoint> polygon,
		IPoint point
	) {
		return ( this as IGeometry ).PointInPolygon( polygon, point.X, point.Y );
	}

	bool IGeometry.PointInPolygon(
		IReadOnlyList<IPoint> polygon,
		int x,
		int y
	) {
		int minX = int.MaxValue;
		int minY = int.MaxValue;
		int maxX = int.MinValue;
		int maxY = int.MinValue;
		for (int i = 0; i < polygon.Count; i++) {
			IPoint point = polygon[i];
			if( point.X < minX ) {
				minX = point.X;
			}
			if( point.X > maxX ) {
				maxX = point.X;
			}

			if( point.Y < minY ) {
				minY = point.Y;
			}
			if( point.Y > maxY ) {
				maxY = point.Y;
			}
		}
		if (x < minX || x > maxX || y < minY || y > maxY) {
			return false;
		}

		// https://wrf.ecse.rpi.edu/Research/Short_Notes/pnpoly.html
		bool inside = false;
		for( int i = 0, j = polygon.Count - 1; i < polygon.Count; j = i++ ) {
			if(
				( polygon[i].Y > y ) != ( polygon[j].Y > y )
				&& x < (( polygon[j].X - polygon[i].X ) * ( y - polygon[i].Y ) / ( polygon[j].Y - polygon[i].Y )) + polygon[i].X
			) {
				inside = !inside;
			}
		}

		return inside;

	}

	void IGeometry.RasterizeLine(
		IPoint p1,
		IPoint p2,
		Action<int, int> callback
	) {
		int x0 = p1.X;
		int y0 = p1.Y;
		int x1 = p2.X;
		int y1 = p2.Y;
		bool steep = Math.Abs( y1 - y0 ) > Math.Abs( x1 - x0 );
		if( steep ) {
			int t;
			t = x0; // swap x0 and y0
			x0 = y0;
			y0 = t;
			t = x1; // swap x1 and y1
			x1 = y1;
			y1 = t;
		}
		if( x0 > x1 ) {
			int t;
			t = x0; // swap x0 and x1
			x0 = x1;
			x1 = t;
			t = y0; // swap y0 and y1
			y0 = y1;
			y1 = t;
		}
		int dx = x1 - x0;
		int dy = Math.Abs( y1 - y0 );
		int error = dx / 2;
		int ystep = ( y0 < y1 ) ? 1 : -1;
		int y = y0;
		for( int x = x0; x <= x1; x++ ) {
			callback( ( steep ? y : x ), ( steep ? x : y ) );
			error -= dy;
			if( error < 0 ) {
				y += ystep;
				error += dx;
			}
		}
	}

	void IGeometry.RasterizePolygon(
		IReadOnlyList<IPoint> polygon,
		Action<int, int> callback
	) {
		int miny = polygon[0].Y;
		int maxy = miny;
		for( int i = 1; i < polygon.Count; i++ ) {
			miny = Math.Min( miny, polygon[i].Y );
			maxy = Math.Max( maxy, polygon[i].Y );
		}

		// Single line polygon
		if( miny == maxy ) {
			int minx = polygon[0].X;
			int maxx = minx;
			for( int i = 1; i < polygon.Count; i++ ) {
				minx = Math.Min( minx, polygon[i].X );
				maxx = Math.Max( maxx, polygon[i].X );
			}

			for( int x = minx; x < maxx; x++ ) {
				callback( x, miny );
			}
		}

		// Regular polygon
		int[] intersect = new int[polygon.Count];
		int previous;
		int x1;
		int x2;
		int y1;
		int y2;
		for( int y = miny; y < maxy; y++ ) {
			int intersections = 0;
			for( int i = 0; i < polygon.Count; i++ ) {
				previous = ( i != 0 ) ? ( i - 1 ) : ( polygon.Count - 1 );

				y1 = polygon[previous].Y;
				y2 = polygon[i].Y;
				if( y1 < y2 ) {
					x1 = polygon[previous].X;
					x2 = polygon[i].X;
				} else if( y1 > y2 ) {
					y2 = polygon[previous].Y;
					y1 = polygon[i].Y;
					x2 = polygon[previous].X;
					x1 = polygon[i].X;
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

		for( int i = 0; i < polygon.Count - 1; i++ ) {
			previous = ( i != 0 ) ? ( i - 1 ) : ( polygon.Count - 1 );
			int y = polygon[i].Y;

			if( ( miny < y ) && ( polygon[previous].Y == y ) && ( y < maxy ) ) {
				int minX = Math.Min( polygon[i].X, polygon[previous].X );
				int count = Math.Abs( polygon[i].X - polygon[previous].X );
				for( int x = minX; x < ( minX + count ); x++ ) {
					callback( x, y );
				}
			}
		}
	}
}
