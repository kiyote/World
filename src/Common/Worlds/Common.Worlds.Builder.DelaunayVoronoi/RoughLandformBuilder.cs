using Common.Geometry.DelaunayVoronoi;

namespace Common.Worlds.Builder.DelaunayVoronoi;

internal sealed class RoughLandformBuilder : IRoughLandformBuilder {

	private readonly IRandom _random;
	private readonly IVoronoiBuilder _voronoiBuilder;

	public RoughLandformBuilder(
		IRandom random,
		IVoronoiBuilder voronoiBuilder
	) {
		_voronoiBuilder = voronoiBuilder;
		_random = random;
	}

	Voronoi IRoughLandformBuilder.Create(
		Size size,
		float landPercentage,
		out List<Cell> roughLandforms
	) {
		Voronoi voronoi = _voronoiBuilder.Create( size, 100 );

		// Get the seeds of the landforms
		List<Cell> cells = voronoi.Cells.Where( c => !c.IsOpen ).ToList();
		int desiredCount = (int)( cells.Count * 0.3 );
		roughLandforms = new List<Cell>();
		do {
			Cell cell = cells[_random.NextInt( cells.Count )];
			roughLandforms.Add( cell );
			cells.Remove( cell );
		} while( roughLandforms.Count < desiredCount );

		// Add the landforms neighbours to beef the shape up
		List<Cell> result = new List<Cell>();
		foreach( Cell seedCell in roughLandforms ) {
			result.Add( seedCell );
			foreach( Cell neighbourCell in voronoi.Neighbours[seedCell].Where( c => !c.IsOpen ) ) {
				if( !result.Contains( neighbourCell ) ) {
					result.Add( neighbourCell );
				}
			}
		}
		roughLandforms = result;

		return voronoi;
	}
}

