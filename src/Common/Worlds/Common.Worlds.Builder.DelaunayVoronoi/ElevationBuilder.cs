using System.Numerics;

namespace Common.Worlds.Builder.DelaunayVoronoi;

internal class ElevationBuilder : IElevationBuilder {

	IReadOnlyDictionary<Cell, float> IElevationBuilder.Create(
		ISize size,
		TectonicPlates tectonicPlates,
		ISearchableVoronoi map,
		IReadOnlySet<Cell> landform
	) {
		Dictionary<Cell, float> result = [];

		HashSet<Edge> collidingEdges = [];
		// Find the cells where plates are colliding and those will be our
		// mountain ranges
		foreach( KeyValuePair<Cell, Vector2> plate in tectonicPlates.Velocity ) {
			IReadOnlyList<Cell> neighbours = tectonicPlates.Plates.Neighbours[plate.Key];
			foreach( Cell neighbour in neighbours ) {
				float dot = Vector2.Dot( plate.Value, tectonicPlates.Velocity[neighbour] );
				if( dot < 0.0f ) {
					// These are colliding plates, so find their shared edges

					foreach( Edge source in plate.Key.Polygon.Edges ) {
						foreach( Edge target in neighbour.Polygon.Edges ) {
							if( source.IsEquivalentTo( target ) ) {
								collidingEdges.Add( source );
							}
						}
					}
				}
			}
		}

		HashSet<Cell> mountainCells = [];
		foreach( Edge edge in collidingEdges ) {
			IReadOnlyList<Cell> cells = map.Search( edge.GetBoundingBox() );

			foreach( Cell cell in cells ) {
				if( cell.Polygon.HasIntersection( edge ) ) {
					mountainCells.Add( cell );
				}
			}
		}

		float currentElevation = 1_000_000; // Arbitrary number to start counting down from
		Queue<Cell> currentQueue = [];
		HashSet<Cell> visited = [];

		foreach( Cell cell in mountainCells ) {
			result[cell] = currentElevation;
			currentQueue.Enqueue( cell );
			visited.Add( cell );
		}

		Queue<Cell> newQueue = [];

		do {
			currentElevation -= 1.0F;
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

		// Normalize the elevation
		currentElevation -= 1.0F;
		foreach( KeyValuePair<Cell, float> item in result ) {
			result[item.Key] = item.Value - currentElevation;
		}

		/*
		float currentElevation = 1_000_000; // Arbitrary number to start counting down from

		// Iterate through the neighbours, decreasing altitude as we go
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
		foreach( IReadOnlySet<Cell> lake in lakes ) {
			float newElevation = int.MaxValue;
			// Find the lowest elevation around the lake
			foreach( Cell cell in lake ) {
				foreach( Cell neighbour in map.Neighbours[cell] ) {
					if( landform.Contains( neighbour ) ) {
						if( result[neighbour] < newElevation ) {
							newElevation = result[neighbour];
						}
					}
				}
			}
			foreach( Cell cell in lake ) {
				result[cell] = newElevation;
				foreach( Cell neighbour in map.Neighbours[cell] ) {
					if( landform.Contains( neighbour ) ) {
						float targetElevation = result[neighbour];
						if (targetElevation < newElevation) {
							result[neighbour] = newElevation;
						}
						if( targetElevation > newElevation ) {
							result[neighbour] = ( targetElevation + newElevation ) / 2.0f;
						}
					}
				}
			}
		}

		// Then re-average the terrain around those tiles
		*/


		return result;
	}
}
