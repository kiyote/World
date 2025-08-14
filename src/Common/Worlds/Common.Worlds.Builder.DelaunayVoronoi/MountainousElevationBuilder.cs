using System.Numerics;

namespace Common.Worlds.Builder.DelaunayVoronoi;

internal class MountainousElevationBuilder : IElevationBuilder {

	IReadOnlyDictionary<Cell, float> IElevationBuilder.Create(
		ISize size,
		TectonicPlates tectonicPlates,
		ISearchableVoronoi map,
		IReadOnlySet<Cell> landform,
		IReadOnlyDictionary<Cell, float> inlandDistance
	) {
		Dictionary<Cell, float> result = [];

		float minimumIntensity = float.MaxValue;
		float maximumIntensity = float.MinValue;
		Dictionary<Edge, float> collidingEdges = [];
		// Find the cells where plates are colliding and those will be our
		// mountain ranges
		foreach( KeyValuePair<Cell, Vector2> plate in tectonicPlates.Velocity ) {
			IReadOnlyList<Cell> neighbours = tectonicPlates.Plates.Neighbours[plate.Key];
			foreach( Cell neighbour in neighbours ) {
				float dot = Vector2.Dot( plate.Value, tectonicPlates.Velocity[neighbour] );
				if( dot < 0.0f ) {
					// These are colliding plates, so find their shared edges
					float intensity = Vector2.Subtract( plate.Value, tectonicPlates.Velocity[neighbour] ).Length();
					// TODO: Figure out the "Velocity" of the impact and adjust the mountain height at that point
					foreach( Edge source in plate.Key.Polygon.Edges ) {
						foreach( Edge target in neighbour.Polygon.Edges ) {
							if( source.IsEquivalentTo( target ) ) {
								if( intensity < minimumIntensity ) {
									minimumIntensity = intensity;
								}
								if( intensity > maximumIntensity ) {
									maximumIntensity = intensity;
								}
								if( collidingEdges.TryGetValue( source, out float value ) ) {
									intensity = Math.Max( intensity, value );
								}
								collidingEdges[source] = intensity;
							}
						}
					}
				}
			}
		}

		// Normalize the intensity from 2->4
		foreach( KeyValuePair<Edge, float> entry in collidingEdges ) {
			collidingEdges[entry.Key] = ( ( ( entry.Value - minimumIntensity ) / maximumIntensity ) + 1.0f ) * 2.0f;
		}

		float minimumElevation = float.MaxValue;
		float maximumElevation = float.MinValue;
		Queue<Cell> currentQueue = [];
		HashSet<Cell> visited = [];
		foreach( KeyValuePair<Edge, float> item in collidingEdges ) {
			IReadOnlyList<Cell> cells = map.Search( item.Key.GetBoundingBox() );

			foreach( Cell cell in cells ) {
				if( cell.Polygon.HasIntersection( item.Key )
					&& inlandDistance.TryGetValue( cell, out float distance )
				) {
					float elevation = distance * item.Value;
					if( elevation < minimumElevation ) {
						minimumElevation = elevation;
					}
					if( elevation > maximumElevation ) {
						maximumElevation = elevation;
					}
					result[cell] = elevation;

					visited.Add( cell );
					foreach( Cell neighbour in map.Neighbours[cell] ) {
						currentQueue.Enqueue( neighbour );
					}
				}
			}
		}

		Queue<Cell> newQueue = [];
		do {
			while( currentQueue.Count > 0 ) {
				Cell currentCell = currentQueue.Dequeue();
				if( visited.Add( currentCell ) ) {
					if( inlandDistance.TryGetValue( currentCell, out float elevation ) ) {
						foreach( Cell neighbour in map.Neighbours[currentCell] ) {
							currentQueue.Enqueue( neighbour );

							if( result.TryGetValue( neighbour, out float neighbourElevation ) ) {
								elevation = ( elevation + neighbourElevation ) / 2.0f;
							}
						}
						result[currentCell] = elevation;
					}
				}
			}
			currentQueue = newQueue;
			newQueue = [];
		} while( currentQueue.Count > 0 );

		return result;
	}
}
