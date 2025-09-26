namespace Common.Worlds.Builder.DelaunayVoronoi;

internal sealed class MapEdgeSaltwaterFinder : ISaltwaterFinder {

	IReadOnlySet<Cell> ISaltwaterFinder.Find(
		ISize size,
		ISearchableVoronoi map,
		IReadOnlySet<Cell> landform
	) {
		Cell start = map.Cells[0];
		bool foundStart = false;

		// Find a cell along the top of the map that isn't land
		Rect topEdge = new Rect( 0, 0, size.Width, 1 );
		IReadOnlyList<Cell> edgeCells = map.Search( topEdge );
		foreach( Cell cell in edgeCells ) {
			if( !landform.Contains( cell ) ) {
				foundStart = true;
				start = cell;
				break;
			}
		}

		if( !foundStart ) {
			// TODO: Search the other edges before barfing
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

