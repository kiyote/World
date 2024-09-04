﻿namespace Common.Worlds.Builder.DelaunayVoronoi;

internal class ElevationBuilder : IElevationBuilder {

	Dictionary<Cell, float> IElevationBuilder.Create(
		ISize size,
		ISearchableVoronoi map,
		HashSet<Cell> landform,
		HashSet<Cell> saltwater,
		HashSet<Cell> freshwater
	) {
		Dictionary<Cell, float> result = [];

		Queue<Cell> currentQueue = [];
		HashSet<Cell> visited = [];

		float currentElevation = 1.0f;
		// Find all the land bordering the ocean
		foreach( Cell cell in saltwater ) {
			foreach( Cell neighbour in map.Neighbours[cell] ) {
				if (!result.ContainsKey(neighbour)
					&& landform.Contains(neighbour)
				) {
					visited.Add( neighbour );
					result[neighbour] = currentElevation;
					currentQueue.Enqueue( neighbour );
				}
			}
		}

		// Iterate through the layers of terrain, increasing the height as we go
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
							result[neighbour] = ( target + currentElevation ) / 2.0f;
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

		// Find the average altitude around lakes and set their coast to be
		// that height

		// Then re-average the terrain around those tiles


		return result;
	}
}
