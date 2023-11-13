namespace Common.Worlds.Builder.DelaunayVoronoi;

internal sealed class MoistureBuilder : IMoistureBuilder {

	public const int CapacityLimit = 1000;

	Dictionary<Cell, float> IMoistureBuilder.Create(
		ISize size,
		ISearchableVoronoi voronoi,
		HashSet<Cell> fineLandforms,
		HashSet<Cell> saltwater,
		HashSet<Cell> freshwater,
		Dictionary<Cell, float> temperature,
		Dictionary<Cell, float> airflow
	) {
		Dictionary<Cell, int> result = new Dictionary<Cell, int>();

		foreach( Cell cell in saltwater ) {
			result[cell] = CapacityLimit;
		}

		foreach( Cell cell in freshwater ) {
			result[cell] = CapacityLimit;
		}

		IReadOnlyList<Cell> sweepEdge = voronoi.Search( 50, 0, 1, size.Height );
		foreach(Cell cell in sweepEdge) {
			result[cell] = CapacityLimit;
		}
		while( sweepEdge.Count > 0 ) {
			sweepEdge = SweepRight( sweepEdge, voronoi, fineLandforms, airflow, temperature, result );
		}

		foreach (Cell cell in voronoi.Cells) {
			if (!result.ContainsKey(cell)) {
				result[cell] = CapacityLimit;
			}
			if( result[cell] == CapacityLimit ) {
				result[cell] = (int)( result[cell] * temperature[cell] );
			}
		}

		// Levelize the result
		int minValue = result.Values.Min();
		foreach( KeyValuePair<Cell, int> pair in result ) {
			result[pair.Key] -= minValue;
		}

		int maxValue = result.Values.Max();
		return result.ToDictionary( r => r.Key, r => (float)( (float)r.Value / (float)maxValue ) );
	}

	private static IReadOnlyList<Cell> SweepRight(
		IReadOnlyList<Cell> source,
		IVoronoi fineVoronoi,
		HashSet<Cell> fineLandforms,
		Dictionary<Cell, float> airflow,
		Dictionary<Cell, float> temperature,
		Dictionary<Cell, int> moisture
	) {
		HashSet<Cell> result = new HashSet<Cell>();
		foreach( Cell cell in source ) {
			int minY = cell.Polygon.Points.Min( p => p.Y );
			int maxY = cell.Polygon.Points.Max( p => p.Y );
			int range = maxY - minY;
			foreach( Cell neighbour in fineVoronoi.Neighbours[cell].Where( n => !n.IsOpen ) ) {
				if( neighbour.Center.X > cell.Center.X
					&& neighbour.Center.Y >= (minY - range)
					&& neighbour.Center.Y <= (maxY + range)
				) {
					result.Add( neighbour );
					if( !moisture.ContainsKey( neighbour ) ) {
						moisture[neighbour] = 0;
					}

					moisture[neighbour] += (int)( moisture[cell] * airflow[cell] );

					if( moisture[neighbour] > CapacityLimit) {
						moisture[neighbour] = CapacityLimit;
					}
				}
			}
		}

		return result.ToList();
	}
}
