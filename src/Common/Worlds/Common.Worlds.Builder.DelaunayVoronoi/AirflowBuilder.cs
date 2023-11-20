namespace Common.Worlds.Builder.DelaunayVoronoi;

internal sealed class AirflowBuilder : IAirflowBuilder {

	private const int CapacityLimit = int.MaxValue / 2;

	Dictionary<Cell, float> IAirflowBuilder.Create(
		ISize size,
		ISearchableVoronoi voronoi,
		HashSet<Cell> fineLandforms,
		HashSet<Cell> mountains,
		HashSet<Cell> hills
	) {
		Dictionary<Cell, int> result = [];

		List<Cell> closedCells = voronoi.Cells.Where( c => !c.IsOpen ).ToList();

		IReadOnlyList<Cell> leftEdge = voronoi.Search( 50, 0, 1, size.Height );
		foreach( Cell cell in leftEdge ) {
			result[cell] = CapacityLimit;
		}
		HashSet<Cell> leewardEdge = GetLeewardNeighbours( voronoi, leftEdge );
		foreach (Cell cell in leewardEdge ) {
			result[cell] = CapacityLimit;
		}
		leewardEdge = GetLeewardNeighbours( voronoi, leewardEdge.ToList() );
		foreach( Cell cell in leewardEdge ) {
			result[cell] = CapacityLimit;
		}

		do {
			leftEdge = SweepRight( leftEdge, voronoi, fineLandforms, mountains, hills, result );
		} while( leftEdge.Count > 0 );

		// Levelize the result
		foreach( Cell cell in voronoi.Cells ) {
			if( !result.ContainsKey( cell ) ) {
				result[cell] = 0;
			}
		}
		int minValue = result.Values.Min();
		int maxValue = result.Values.Max();
		foreach( KeyValuePair<Cell, int> pair in result ) {
			result[pair.Key] = pair.Value - minValue;
		}

		return result.ToDictionary( r => r.Key, r => (float)((float)r.Value / (float)maxValue) );
	}

	private static IReadOnlyList<Cell> SweepRight(
		IReadOnlyList<Cell> current,
		IVoronoi fineVoronoi,
		HashSet<Cell> fineLandforms,
		HashSet<Cell> mountains,
		HashSet<Cell> hills,
		Dictionary<Cell, int> airflow
	) {
		HashSet<Cell> result = [];
		foreach( Cell cell in current ) {

			double giveaway = airflow[cell];
			if( fineLandforms.Contains( cell ) ) {
				if( mountains.Contains( cell ) ) {
					giveaway *= 0.01;
				} else if( hills.Contains( cell ) ) {
					giveaway *= 0.05;
				} else {
					giveaway *= 0.0875;
				}
			} else {
				giveaway *= 0.0975;
			}

			foreach( Cell neighbour in fineVoronoi.Neighbours[cell].Where( n => !n.IsOpen ) ) {
				if( neighbour.Center.X > cell.Center.X ) {
					if( !airflow.TryGetValue( neighbour, out int value ) ) {
						value = 0;
						airflow[neighbour] = value;
					}
					// Add the giveaway to the neighbour
					int newValue = (int)giveaway + value;
					if ( newValue > CapacityLimit) {
						newValue = CapacityLimit;
					}

					// Clamp the value the neighbour will have
					if( fineLandforms.Contains( neighbour ) ) {
						if( mountains.Contains( neighbour ) ) {
							newValue = (int)(newValue * 0.1);
						} else if( hills.Contains( neighbour ) ) {
							newValue = (int)(newValue * 0.5);
						} else {
							newValue = (int)(newValue * 0.95);
						}
					}

					// That's the amount of flow
					airflow[neighbour] = newValue;

					result.Add( neighbour );
				}
			}
		}
		return result.ToList();
	}

	private static HashSet<Cell> GetLeewardNeighbours(
		IVoronoi fineVoronoi,
		IReadOnlyList<Cell> source
	) {
		HashSet<Cell> result = [];
		foreach (Cell cell in source) {
			foreach (Cell neighbour in fineVoronoi.Neighbours[cell].Where( c => !c.IsOpen )) {
				if (neighbour.Center.X > cell.Center.X) {
					result.Add( neighbour );
				}
			}
		}

		return result;
	}
}
