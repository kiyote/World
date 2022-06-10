namespace Common.Geometry.DelaunayVoronoi;

[System.Diagnostics.CodeAnalysis.SuppressMessage( "Performance", "CA1812:An internal (assembly-level) type is never instantiated.", Justification = "This class is instantiated via DI." )]
internal sealed class VoronoiEdgeDetector : IVoronoiEdgeDetector {

	private readonly IGeometry _geometry;

	public VoronoiEdgeDetector(
		IGeometry geometry
	) {
		_geometry = geometry;
	}

	HashSet<Cell> IVoronoiEdgeDetector.Find(
		Size size,
		Voronoi voronoi,
		VoronoiEdge edge
	) {
		HashSet<Cell> result = new HashSet<Cell>();

		List<Cell> closedCells = voronoi.Cells.Where( c => !c.IsOpen ).ToList();

		switch( edge ) {
			case VoronoiEdge.Left: {
					int margin = ( size.Rows / 100 );
					int r = margin;
					while( r < ( size.Rows - margin ) ) {
						for( int c = 0; c < size.Columns; c++ ) {
							Cell? cell = closedCells.FirstOrDefault( check => _geometry.PointInPolygon( check.Points, c, r ) );
							if( cell is not null ) {
								result.Add( cell );
								r = cell.Points.Max( p => p.Y );
								break;
							}
						}
						r++;
					}
				}
				break;
			default:
				throw new NotImplementedException();

		}

		return result;
	}
}
