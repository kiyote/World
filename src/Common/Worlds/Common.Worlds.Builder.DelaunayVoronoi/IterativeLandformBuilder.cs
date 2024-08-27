
namespace Common.Worlds.Builder.DelaunayVoronoi;

internal sealed class IterativeLandformBuilder : ILandformBuilder {

	private readonly IRandom _random;
	private readonly IVoronoiBuilder _voronoiBuilder;

	public IterativeLandformBuilder(
		IRandom random,
		IVoronoiBuilder voronoiBuilder
	) {
		_random = random;
		_voronoiBuilder = voronoiBuilder;
	}

	HashSet<Cell> ILandformBuilder.Create(
		ISize size,
		out ISearchableVoronoi voronoi
	) {
		int cellSize = Math.Min( size.Width, size.Height ) / 20;
		HashSet<Cell> landforms = CreateLandform( size, cellSize );
		voronoi = _voronoiBuilder.Create( size, cellSize );

		for( int distance = cellSize / 2; distance >= 5; distance /= 2 ) {
			// Create the finer landforms
			voronoi = _voronoiBuilder.Create( size, distance );

			HashSet<Cell> finerLandforms = [];
			foreach( Cell roughCell in landforms ) {
				// Find all the fine cells that fall within the bounding box
				// of the rough cell
				Rect bounds = roughCell.BoundingBox;
				IReadOnlyList<Cell> fineCells = voronoi.Search( bounds );

				foreach( Cell fineCell in fineCells ) {
					if( roughCell.Polygon.Contains( fineCell.Center ) ) {
						// Only make this land if all of its neighbours are closed,
						// otherwise you'll have land with an Open water neighbour
						// which leads to weird degenerate cases when trying to
						// render the coast.
						bool openNeighbours = voronoi.Neighbours[fineCell].Any( c => c.IsOpen );
						if( !openNeighbours ) {
							finerLandforms.Add( fineCell );
						}
					}
				}
			}

			landforms = finerLandforms;
		}

		return landforms;
	}

	private HashSet<Cell> CreateLandform(
		ISize size,
		int cellSize
	) {
		IVoronoi roughVoronoi = _voronoiBuilder.Create( size, cellSize );

		// Get the seeds of the landforms
		List<Cell> cells = roughVoronoi.Cells.Where( c => !c.IsOpen ).ToList();
		int desiredCount = (int)( cells.Count * 0.15 );
		HashSet<Cell> roughLandforms = [];
		while ( roughLandforms.Count < desiredCount ) {
			Point location = new Point( _random.NextInt( size.Width ), _random.NextInt( size.Height ) );
			Cell? target = null;
			foreach (Cell cell in cells) {
				if (cell.Polygon.Contains(location)) {
					target = cell;
					break;
				}
			}
			if (target is not null) {
				roughLandforms.Add( target );
				cells.Remove( target );
			}
		}

		/*
	do {
		Cell cell = cells[_random.NextInt( cells.Count )];

		roughLandforms.Add( cell );
		cells.Remove( cell );
	} while( roughLandforms.Count < desiredCount );
		*/

		// Add the landforms neighbours to beef the shape up
		HashSet<Cell> result = [];
		foreach( Cell seedCell in roughLandforms ) {
			result.Add( seedCell );
			foreach( Cell neighbourCell in roughVoronoi.Neighbours[seedCell].Where( c => !c.IsOpen ) ) {
				result.Add( neighbourCell );
			}
		}
		roughLandforms = result;

		return roughLandforms;
	}
}
