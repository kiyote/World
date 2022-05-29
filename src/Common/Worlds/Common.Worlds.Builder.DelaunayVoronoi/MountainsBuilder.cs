using Common.Geometry;

namespace Common.Worlds.Builder.DelaunayVoronoi;

internal sealed class MountainsBuilder : IMountainsBuilder {

	private readonly IRandom _random;
	private readonly IGeometry _geometry;

	public MountainsBuilder(
		IRandom random,
		IGeometry geometry
	) {
		_random = random;
		_geometry = geometry;
	}


	HashSet<Cell> IMountainsBuilder.Create(
		Size size,
		Voronoi fineVoronoi,
		HashSet<Cell> fineLandforms
	) {
		HashSet<Cell> result = new HashSet<Cell>();
		do {
			List<Edge> lines = GetMountainLines( size, size.Columns / 100 );
			List<Cell> rangeCells = BuildRanges( fineVoronoi, fineLandforms, lines );
			foreach( Cell cell in rangeCells ) {
				result.Add( cell );
			};
		} while( result.Count < size.Columns / 12 );
		return result;
	}

	public List<Edge> GetMountainLines(
		Size size,
		int count
	) {
		List<Edge> result = new List<Edge>();

		// Get a rough direction for the mountain range
		float direction = _random.NextInt( 90 ) - 45;
		// Start it somewhere middle-top -ish
		int widthRange = size.Columns / 2;
		Point start = new Point(
			_random.NextInt( widthRange ) + ( widthRange / 2 ),
			_random.NextInt( size.Rows )
		);
		int segmentRange = ( size.Columns / 12 );

		do {
			int segmentLength = _random.NextInt( segmentRange ) + ( segmentRange / 2 );
			Point next = new Point(
				start.X + (int)( Math.Sin( Math.PI / 180 * direction ) * segmentLength ),
				start.Y + (int)( Math.Cos( Math.PI / 180 * direction ) * segmentLength )
			);
			result.Add( new Edge( start, next ) );
			start = next;
			// This allows the mountain range to be jagged
			direction += ( _random.NextInt( 60 ) - 30 );

		} while( result.Count < count );

		return result;
	}

	public List<Cell> BuildRanges(
		Voronoi fineVoronoi,
		HashSet<Cell> fineLandforms,
		List<Edge> lines
	) {
		List<Cell> result = new List<Cell>();
		// We select 3/4 of the lines at random in order to possibly have
		// gaps in the mountain range
		lines = lines
			.OrderBy( l => _random.NextInt() ) // Shuffle the list
			.Take( (int)( lines.Count * 0.75 ) ) // Take 3/4 of the list
			.ToList();

		// Check to see if the line crosses a cell, if it does, mark it as a mountain
		foreach( Edge edge in lines ) {
			_geometry.RasterizeLine(
				edge.A,
				edge.B,
				( int x, int y ) => {
					foreach( Cell fineCell in fineLandforms ) {
						if( _geometry.PointInPolygon( fineCell.Points, x, y ) ) {
							if( !result.Contains( fineCell ) ) {
								result.Add( fineCell );
							}
						}
					}
				}
			);
		}

		// This prevents the mountains from being up against water
		result = result.Where( mountainCell => {
			bool allowed = true;
			foreach( Cell neighbourCell in fineVoronoi.Neighbours[mountainCell] ) {
				if( !fineLandforms.Contains( neighbourCell ) ) {
					allowed = false;
				}
			}
			return allowed;
		} ).ToList();

		return result;
	}
}
