using Common.Geometry;

namespace Common.Worlds.Builder.DelaunayVoronoi;

internal sealed class AirflowBuilder : IAirflowBuilder {

	private const int CapacityLimit = int.MaxValue / 2;
	private readonly IVoronoiEdgeDetector _voronoiEdgeDetector;

	public AirflowBuilder(
		IVoronoiEdgeDetector voronoiEdgeDetector
	) {
		_voronoiEdgeDetector = voronoiEdgeDetector;
	}

	Dictionary<Cell, float> IAirflowBuilder.Create(
		Size size,
		Voronoi fineVoronoi,
		HashSet<Cell> fineLandforms,
		HashSet<Cell> mountains,
		HashSet<Cell> hills
	) {
		Dictionary<Cell, int> result = new Dictionary<Cell, int>();

		List<Cell> closedCells = fineVoronoi.Cells.Where( c => !c.IsOpen ).ToList();

		HashSet<Cell> leftEdge = _voronoiEdgeDetector.Find( size, fineVoronoi, VoronoiEdge.Left );
		foreach( Cell cell in leftEdge ) {
			result[cell] = CapacityLimit;
		}
		HashSet<Cell> leewardEdge = GetLeewardNeighbours( fineVoronoi, leftEdge );
		foreach (Cell cell in leewardEdge ) {
			result[cell] = CapacityLimit;
		}
		leewardEdge = GetLeewardNeighbours( fineVoronoi, leewardEdge );
		foreach( Cell cell in leewardEdge ) {
			result[cell] = CapacityLimit;
		}

		do {
			leftEdge = SweepRight( leftEdge, fineVoronoi, fineLandforms, mountains, hills, result );
		} while( leftEdge.Count > 0 );

		// Levelize the result
		foreach( Cell cell in fineVoronoi.Cells ) {
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

	private static HashSet<Cell> SweepRight(
		HashSet<Cell> current,
		Voronoi fineVoronoi,
		HashSet<Cell> fineLandforms,
		HashSet<Cell> mountains,
		HashSet<Cell> hills,
		Dictionary<Cell, int> airflow
	) {
		HashSet<Cell> result = new HashSet<Cell>();
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
					if( !airflow.ContainsKey( neighbour ) ) {
						airflow[neighbour] = 0;
					}
					// Add the giveaway to the neighbour
					int newValue = (int)giveaway + airflow[neighbour];
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
		return result;
	}

	private static HashSet<Cell> GetLeewardNeighbours(
		Voronoi fineVoronoi,
		HashSet<Cell> source
	) {
		HashSet<Cell> result = new HashSet<Cell>();
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
