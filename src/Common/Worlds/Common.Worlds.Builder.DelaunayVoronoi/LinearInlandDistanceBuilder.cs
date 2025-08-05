namespace Common.Worlds.Builder.DelaunayVoronoi;

internal sealed class LinearInlandDistanceBuilder : IInlandDistanceBuilder {
	IReadOnlyDictionary<Cell, float> IInlandDistanceBuilder.Create(
		ISize size,
		ISearchableVoronoi map,
		IReadOnlySet<Cell> landform,
		IReadOnlySet<Cell> coast
	) {
		Dictionary<Cell, float> result = [];
		HashSet<Cell> visited = [];
		float currentElevation = 1.0f;
		Queue<Cell> currentQueue = [];

		// Find all of the land adjacent to the coastline and set it to 1
		foreach( Cell coastCell in coast ) {
			IReadOnlyList<Cell> neighbours = map.Neighbours[coastCell];
			foreach ( Cell neighbourCell in neighbours) {
				if( !result.ContainsKey( neighbourCell )
					&& landform.Contains( neighbourCell )
				) {
					visited.Add( neighbourCell );
					result[neighbourCell] = currentElevation;
					currentQueue.Enqueue( neighbourCell );
				}
			}
		}

		// For every land cell discovered, iterate over them finding their
		// neighbour cells, and setting their distance to the ocean equal to
		// the current distance, or the distance already calculated, whichever
		// is smaller
		Queue<Cell> newQueue = [];
		do {
			currentElevation += 1.0F;
			while( currentQueue.Count > 0 ) {
				Cell cell = currentQueue.Dequeue();
				foreach( Cell neighbour in map.Neighbours[cell] ) {
					if( landform.Contains( neighbour )
						&& !visited.Contains( neighbour )
					) {
						newQueue.Enqueue( neighbour );
						if( result.TryGetValue( neighbour, out float target ) ) {
							result[neighbour] = Math.Min( target, currentElevation );
						} else {
							result[neighbour] = currentElevation;
						}
						visited.Add( neighbour );
					}
				}
			}
			currentQueue = newQueue;
			newQueue = [];
		} while( currentQueue.Count > 0 );

		return result;
	}
}
