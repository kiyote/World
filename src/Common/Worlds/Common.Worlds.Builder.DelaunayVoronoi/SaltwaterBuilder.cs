namespace Common.Worlds.Builder.DelaunayVoronoi;

internal sealed class SaltwaterBuilder : ISaltwaterBuilder {

	HashSet<Cell> ISaltwaterBuilder.Create(
		ISize size,
		IVoronoi fineVoronoi,
		HashSet<Cell> fineLandforms
	) {
		// Find a cell along the top of the map that isn't land
		Cell start = fineVoronoi.Cells[0];
		for( int i = 0; i < size.Width; i += 10 ) {
			foreach( Cell cell in fineVoronoi.Cells ) {
				if( cell.Polygon.Contains( i, 0 ) ) {
					if( !fineLandforms.Contains( cell ) ) {
						start = cell;
					}
					break;
				}
			}
		}

		HashSet<Cell> saltwater = [];

		Queue<Cell> queue = new Queue<Cell>();
		queue.Enqueue( start );

		List<Cell> visited = [];
		while( queue.Count != 0 ) {
			Cell cell = queue.Dequeue();
			if( !visited.Contains( cell ) ) {
				visited.Add( cell );

				foreach( Cell neighbour in fineVoronoi.Neighbours[cell] ) {
					if( !fineLandforms.Contains( neighbour ) ) {
						queue.Enqueue( neighbour );
					}
				}
				saltwater.Add( cell );
			}
		}

		return saltwater;
	}
}

