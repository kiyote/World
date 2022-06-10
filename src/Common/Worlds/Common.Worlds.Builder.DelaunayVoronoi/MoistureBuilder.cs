namespace Common.Worlds.Builder.DelaunayVoronoi;

internal sealed class MoistureBuilder : IMoistureBuilder {

	public const int CapacityLimit = 1000;
	private readonly IVoronoiEdgeDetector _voronoiEdgeDetector;

	public MoistureBuilder(
		IVoronoiEdgeDetector voronoiEdgeDetector
	) {
		_voronoiEdgeDetector = voronoiEdgeDetector;
	}

	Dictionary<Cell, float> IMoistureBuilder.Create(
		Size size,
		Voronoi fineVoronoi,
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

		HashSet<Cell> sweepEdge = _voronoiEdgeDetector.Find( size, fineVoronoi, VoronoiEdge.Left );
		while( sweepEdge.Count > 0 ) {
			sweepEdge = SweepRight( sweepEdge, fineVoronoi, fineLandforms, airflow, temperature, result );
		}

		foreach (Cell cell in fineVoronoi.Cells) {
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

	private static HashSet<Cell> SweepRight(
		HashSet<Cell> source,
		Voronoi fineVoronoi,
		HashSet<Cell> fineLandforms,
		Dictionary<Cell, float> airflow,
		Dictionary<Cell, float> temperature,
		Dictionary<Cell, int> moisture
	) {
		HashSet<Cell> result = new HashSet<Cell>();
		foreach( Cell cell in source ) {
			int minY = cell.Points.Min( p => p.Y );
			int maxY = cell.Points.Max( p => p.Y );
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

		return result;
	}
}
