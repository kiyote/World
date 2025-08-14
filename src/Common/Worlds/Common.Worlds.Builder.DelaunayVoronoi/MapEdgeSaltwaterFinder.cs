namespace Common.Worlds.Builder.DelaunayVoronoi;

internal sealed class MapEdgeSaltwaterFinder : ISaltwaterFinder {

	IReadOnlySet<Cell> ISaltwaterFinder.Find(
		ISize size,
		IVoronoi map,
		IReadOnlySet<Cell> landform
	) {
		// Find a cell along the top of the map that isn't land
		Cell start = map.Cells[0];
		bool foundStart = false;
		for( int i = 0; i < size.Width; i += 10 ) {
			foreach( Cell cell in map.Cells ) {
				if( !landform.Contains( cell )
					&& cell.Polygon.Contains( i, 0 )
				) {
					foundStart = true;
					start = cell;
					break;
				}
			}
		}
		if( !foundStart ) {
			throw new InvalidOperationException( "Unable to find non-land at top edge of map." );
		}

		HashSet<Cell> saltwater = [];

		Queue<Cell> queue = new Queue<Cell>();
		queue.Enqueue( start );

		HashSet<Cell> visited = [];
		while( queue.Count != 0 ) {
			// From the starting cell, check to see if we've been here before...
			Cell cell = queue.Dequeue();
			if( visited.Add( cell ) ) {
				// Now add every neighbour of the cell if it's not land
				foreach( Cell neighbour in map.Neighbours[cell] ) {
					if( !landform.Contains( neighbour ) ) {
						queue.Enqueue( neighbour );
					}
				}
				// We know this cell isn't land, so it's saltwater
				saltwater.Add( cell );
			}
		}

		return saltwater;
	}
}

