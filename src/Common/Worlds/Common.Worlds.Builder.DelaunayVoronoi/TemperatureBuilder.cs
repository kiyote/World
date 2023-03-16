using Common.Geometry;

namespace Common.Worlds.Builder.DelaunayVoronoi;

internal sealed class TemperatureBuilder : ITemperatureBuilder {

	Dictionary<Cell, float> ITemperatureBuilder.Create(
		Size size,
		ISearchableVoronoi voronoi,
		HashSet<Cell> fineLandforms,
		HashSet<Cell> mountains,
		HashSet<Cell> hills,
		HashSet<Cell> oceans,
		HashSet<Cell> lakes
	) {
		int midLine = size.Rows / 2;

		Dictionary<Cell, int> result = new Dictionary<Cell, int>();
		foreach( Cell cell in voronoi.Cells.Where( c => !c.IsOpen ) ) {
			result[cell] = int.MaxValue;
		}

		int heat = 1;
		HashSet<Cell> current = RenderEquator( midLine, size, voronoi, result );
		do {
			current = HeatNeighbours( voronoi, current, result, heat );
		} while( current.Count > 0 );

		int maxValue = result.Values.Max();
		int minValue = result.Values.Min();
		int range = maxValue - minValue;
		foreach( KeyValuePair<Cell, int> entry in result ) {
			result[entry.Key] = entry.Value - minValue;
		}

		foreach( Cell land in fineLandforms ) {
			result[land] = result[land] + ( result[land] / 10 );
		}

		foreach( Cell mountain in mountains ) {
			result[mountain] = result[mountain] / 5;
		}

		foreach( Cell hill in hills ) {
			result[hill] = result[hill] / 3;
		}

		// Levelize the result
		bool tempMissing;
		do {
			tempMissing = false;
			foreach( Cell cell in voronoi.Cells.Where( c => c.IsOpen ) ) {
				List<Cell> neighbours = voronoi.Neighbours[cell].Where( c => result.ContainsKey( c ) ).ToList();
				if( neighbours.Any() ) {
					result[cell] = (int)neighbours.Average( n => result[n] );
				} else {
					tempMissing = true;
				}
			}

		} while( tempMissing );

		minValue = result.Values.Min();
		maxValue = result.Values.Max();
		foreach( KeyValuePair<Cell, int> pair in result ) {
			result[pair.Key] -= minValue;
		}
		return result.ToDictionary( r => r.Key, r => (float)( (float)r.Value / (float)maxValue ) );
	}

	private static HashSet<Cell> RenderEquator(
		int midLine,
		Size size,
		ISearchableVoronoi voronoi,
		Dictionary<Cell, int> temperatureMap
	) {
		IReadOnlyList<Cell> equatorCells = voronoi.Search( 0, midLine, size.Columns, 1 );
		foreach (Cell cell in equatorCells) {
			temperatureMap[cell] = int.MaxValue - 1;			
		}
		return equatorCells.ToHashSet();
	}

	private static HashSet<Cell> HeatNeighbours(
		IVoronoi fineVoronoi,
		HashSet<Cell> current,
		Dictionary<Cell, int> result,
		int heat
	) {
		HashSet<Cell> neighbours = new HashSet<Cell>();
		foreach( Cell source in current ) {
			foreach( Cell neighbour in fineVoronoi.Neighbours[source].Where( c => !c.IsOpen ) ) {
				if( result[neighbour] == int.MaxValue ) {
					if( neighbour.Center.Y < source.Center.Y
						|| neighbour.Center.Y > source.Center.Y
					) {
						result[neighbour] = result[source] - heat;

					} else {
						result[neighbour] = result[source];
					}
					neighbours.Add( neighbour );
				}
			}
		}
		return neighbours;
	}
}
