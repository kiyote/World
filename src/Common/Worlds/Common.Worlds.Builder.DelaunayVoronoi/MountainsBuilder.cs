using Kiyote.Geometry.Rasterizers;

namespace Common.Worlds.Builder.DelaunayVoronoi;

internal sealed class MountainsBuilder : IMountainsBuilder {

	private readonly IRandom _random;
	private readonly IRasterizer _rasterizer;

	public MountainsBuilder(
		IRandom random,
		IRasterizer rasterizer
	) {
		_random = random;
		_rasterizer = rasterizer;
	}


	HashSet<Cell> IMountainsBuilder.Create(
		ISize size,
		ISearchableVoronoi voronoi,
		HashSet<Cell> fineLandforms
	) {
		HashSet<Cell> result = new HashSet<Cell>();
		do {
			List<Edge> lines = GetMountainLines( size, size.Width / 100 );
			HashSet<Cell> rangeCells = BuildRanges( voronoi, fineLandforms, lines );
			foreach( Cell cell in rangeCells ) {
				result.Add( cell );
			};
		} while( result.Count < size.Width / 12 );
		return result;
	}

	public List<Edge> GetMountainLines(
		ISize size,
		int count
	) {
		List<Edge> result = new List<Edge>();

		// Get a rough direction for the mountain range
		float direction = _random.NextInt( 90 ) - 45;
		// Start it somewhere middle-top -ish
		int widthRange = size.Width / 2;
		Point start = new Point(
			_random.NextInt( widthRange ) + ( widthRange / 2 ),
			_random.NextInt( size.Height )
		);
		int segmentRange = ( size.Width / 12 );

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

	public HashSet<Cell> BuildRanges(
		ISearchableVoronoi voronoi,
		HashSet<Cell> fineLandforms,
		List<Edge> lines
	) {
		HashSet<Cell> result = new HashSet<Cell>();
		// We select 3/4 of the lines at random in order to possibly have
		// gaps in the mountain range
		lines = lines
			.OrderBy( l => _random.NextInt() ) // Shuffle the list
			.Take( (int)( lines.Count * 0.75 ) ) // Take 3/4 of the list
			.ToList();


		// Check to see if the line crosses a cell, if it does, mark it as a mountain
		foreach( Edge edge in lines ) {
			Rect edgeBounds = new Rect( edge.A, edge.B );
			IReadOnlyList<Cell> targetCells = voronoi.Search( edgeBounds );
			
			_rasterizer.Rasterize(
				edge.A,
				edge.B,
				( int x, int y ) => {
					foreach( Cell cell in targetCells ) {
						if (cell.Polygon.Contains( x, y )) { 
							result.Add( cell );
						}
					}
				}
			);
		}

		// This prevents the mountains from being up against water
		result = result.Where( mountainCell => {
			bool allowed = true;
			foreach( Cell neighbourCell in voronoi.Neighbours[mountainCell] ) {
				if( !fineLandforms.Contains( neighbourCell ) ) {
					allowed = false;
				}
			}
			return allowed;
		} ).ToHashSet();

		return result;
	}
}
