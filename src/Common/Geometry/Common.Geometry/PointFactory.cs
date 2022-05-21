using Common.Core;

namespace Common.Geometry;

[System.Diagnostics.CodeAnalysis.SuppressMessage( "Performance", "CA1812:An internal (assembly-level) type is never instantiated.", Justification = "This class is instantiated via DI." )]
internal sealed class PointFactory : IPointFactory {

	private readonly IRandom _random;

	public PointFactory(
		IRandom random
	) {
		_random = random;
	}

	IReadOnlyList<IPoint> IPointFactory.Random(
		int count,
		Size bounds,
		int distanceApart
	) {
		List<Point> result = new List<Point>( count );
		do {
			int x = _random.NextInt( bounds.Columns );
			int y = _random.NextInt( bounds.Rows );

			bool valid = true;
			for (int i = 0; i < result.Count; i++) {
				Point p = result[i];
				if (Math.Abs(p.X - x) <= distanceApart
					&& Math.Abs(p.Y - y) <= distanceApart
				) {
					valid = false;
				}
			}
			if (valid) {
				result.Add( new Point( x, y ) );
			}

		} while( result.Count < count );


		return result;
	}
}

