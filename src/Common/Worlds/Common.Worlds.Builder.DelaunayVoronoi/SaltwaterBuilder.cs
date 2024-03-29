﻿namespace Common.Worlds.Builder.DelaunayVoronoi;

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
			// From the starting cell, check to see if we've been here before...
			Cell cell = queue.Dequeue();
			if( !visited.Contains( cell ) ) {
				// If we haven't, mark that we have
				visited.Add( cell );

				// Now add every neighbour of the cell if it's not land
				foreach( Cell neighbour in fineVoronoi.Neighbours[cell] ) {
					if( !fineLandforms.Contains( neighbour ) ) {
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

