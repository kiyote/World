using System.Numerics;
using Kiyote.Geometry.Noises;

namespace Common.Worlds.Builder.DelaunayVoronoi;

internal class MountainousElevationBuilder : IElevationBuilder {

	private readonly INoisyEdgeFactory _noisyEdgeFactory;

	public MountainousElevationBuilder(
		INoisyEdgeFactory noisyEdgeFactory
	) {
		_noisyEdgeFactory = noisyEdgeFactory;
	}

	Task<IReadOnlyDictionary<Cell, float>> IElevationBuilder.CreateAsync(
		ISize size,
		TectonicPlates tectonicPlates,
		ISearchableVoronoi map,
		IReadOnlySet<Cell> landform,
		IReadOnlyDictionary<Cell, float> inlandDistance,
		CancellationToken cancellationToken
	) {
		Dictionary<Cell, float> result = [];

		float minimumIntensity = float.MaxValue;
		float maximumIntensity = float.MinValue;
		Dictionary<Edge, float> collidingEdges = [];
		Dictionary<Edge, Edge> controls = [];
		HashSet<Edge> mountainEdges = [];  // Dedupes processed edges

		// Find the cells where plates are colliding and those will be our
		// mountain ranges
		foreach( KeyValuePair<Cell, Vector2> plate in tectonicPlates.Velocity ) {

			cancellationToken.ThrowIfCancellationRequested();

			IReadOnlyList<Cell> neighbours = tectonicPlates.Plates.Neighbours[plate.Key];
			foreach( Cell neighbour in neighbours ) {
				float dot = Vector2.Dot( plate.Value, tectonicPlates.Velocity[neighbour] );
				if( dot < 0.0f ) {
					// These are colliding plates, so find their shared edges
					float intensity = Math.Abs( dot );
					foreach( Edge source in plate.Key.Polygon.Edges ) {
						foreach( Edge target in neighbour.Polygon.Edges ) {
							if( 
								!mountainEdges.Contains( source )
								&& !mountainEdges.Contains( target )
								&& source.IsEquivalentTo( target )
							) {
								mountainEdges.Add( source );
								mountainEdges.Add( target );
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
								controls[source] = new Edge( plate.Key.Center, neighbour.Center );
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
			Edge edge = item.Key;
			float intensity = item.Value;
			float magnitude = edge.Magnitude();

			if( magnitude > 32 ) {
				NoisyEdge noisyEdge = _noisyEdgeFactory.Create( edge, controls[edge], 0.5f, 4 );

				IReadOnlyList<Cell> cells = map.Search( edge.GetBoundingBox() );

				foreach( Cell cell in cells ) {

					cancellationToken.ThrowIfCancellationRequested();

					foreach( Edge noiseEdge in noisyEdge.Noise ) {
						if( cell.Polygon.HasIntersection( noiseEdge )
							&& inlandDistance.TryGetValue( cell, out float distance )
						) {
							float elevation = distance * intensity;
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
			} else {
				IReadOnlyList<Cell> cells = map.Search( edge.GetBoundingBox() );

				foreach( Cell cell in cells ) {

					cancellationToken.ThrowIfCancellationRequested();

					if( cell.Polygon.HasIntersection( edge )
						&& inlandDistance.TryGetValue( cell, out float distance )
					) {
						float elevation = distance * intensity;
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

			cancellationToken.ThrowIfCancellationRequested();

		} while( currentQueue.Count > 0 );

		return Task.FromResult<IReadOnlyDictionary<Cell, float>>( result);
	}
}
