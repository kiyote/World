using Common.Geometry;
using Common.Geometry.DelaunayVoronoi;

namespace Common.Worlds.Builder.DelaunayVoronoi;

internal sealed class LandformBuilder : ILandformBuilder {

	private readonly IRandom _random;
	private readonly IGeometry _geometry;
	private readonly IVoronoiBuilder _voronoiBuilder;

	public LandformBuilder(
		IRandom random,
		IGeometry geometry,
		IVoronoiBuilder voronoiBuilder
	) {
		_random = random;
		_geometry = geometry;
		_voronoiBuilder = voronoiBuilder;
	}

	HashSet<Cell> ILandformBuilder.Create(
		Size size,
		out Voronoi voronoi
	) {
		HashSet<Cell> roughLandforms = CreateRoughLandform( size );
		HashSet<Cell> fineLandforms = CreateFineLandform( roughLandforms, size, out voronoi );

		return fineLandforms;
	}

	private HashSet<Cell> CreateRoughLandform(
		Size size
	) {
		Voronoi roughVoronoi = _voronoiBuilder.Create( size, 100 );

		// Get the seeds of the landforms
		List<Cell> cells = roughVoronoi.Cells.Where( c => !c.IsOpen ).ToList();
		int desiredCount = (int)( cells.Count * 0.3 );
		HashSet<Cell> roughLandforms = new HashSet<Cell>();
		do {
			Cell cell = cells[_random.NextInt( cells.Count )];
			roughLandforms.Add( cell );
			cells.Remove( cell );
		} while( roughLandforms.Count < desiredCount );

		// Add the landforms neighbours to beef the shape up
		HashSet<Cell> result = new HashSet<Cell>();
		foreach( Cell seedCell in roughLandforms ) {
			result.Add( seedCell );
			foreach( Cell neighbourCell in roughVoronoi.Neighbours[seedCell].Where( c => !c.IsOpen ) ) {
				result.Add( neighbourCell );
			}
		}
		roughLandforms = result;

		return roughLandforms;
	}

	private HashSet<Cell> CreateFineLandform(
		HashSet<Cell> roughLandforms,
		Size size,
		out Voronoi voronoi
	) {
		int fineCount = size.Columns * size.Rows / 200;
		voronoi = _voronoiBuilder.Create( size, fineCount );

		HashSet<Cell> fineLandforms = new HashSet<Cell>();
		foreach( Cell fineCell in voronoi.Cells.Where( c => !c.IsOpen ) ) {
			foreach( Cell roughCell in roughLandforms ) {
				if( _geometry.PointInPolygon( roughCell.Points, fineCell.Center ) ) {
					// Only make this land if all of its neighbours are closed,
					// otherwise you'll have land with an Open water neighbour
					// which leads to weird degenerate cases when trying to
					// render the coast.
					bool openNeighbours = voronoi.Neighbours[fineCell].Any( c => c.IsOpen );
					if( !openNeighbours ) {
						fineLandforms.Add( fineCell );
					}
				}
			}
		}

		return fineLandforms;
	}
}

