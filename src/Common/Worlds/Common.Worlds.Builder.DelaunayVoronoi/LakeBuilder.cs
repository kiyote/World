namespace Common.Worlds.Builder.DelaunayVoronoi;

internal class LakeBuilder : ILakeBuilder {

	List<HashSet<Cell>> ILakeBuilder.Create(
		ISize size,
		IVoronoi map,
		HashSet<Cell> landform,
		HashSet<Cell> saltwater,
		HashSet<Cell> freshwater
	) {
		List<HashSet<Cell>> result = [];
		HashSet<Cell> visited = [];
		foreach( Cell cell in freshwater ) {
			// If we've never seen this freshwater cell before it will be part
			// of a lake we've never visited
			if( visited.Add( cell ) ) {
				// Start tracking the lake
				HashSet<Cell> lake = [];
				Queue<Cell> queue = [];
				queue.Enqueue( cell );
				while( queue.Count > 0 ) {
					Cell target = queue.Dequeue();
					// Any cell in the queue is part of the lake
					lake.Add( target );
					// Check the cells neighbours to see if they're also
					// freshwater and never seen before
					foreach( Cell neighbour in map.Neighbours[target] ) {
						if (visited.Add( neighbour ) ) {
							// If the neighbour cell is freshwater then it's
							// part of the lake and we need to explore _it's_
							// neighbours
							if( freshwater.Contains( neighbour ) ) {
								lake.Add( neighbour );
								queue.Enqueue( neighbour );
							}
						}
					}
				}
				result.Add( lake );
			}
		}

		return result;
	}
}
