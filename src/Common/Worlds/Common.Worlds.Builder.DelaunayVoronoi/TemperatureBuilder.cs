using Common.Geometry;

namespace Common.Worlds.Builder.DelaunayVoronoi;

internal sealed class TemperatureBuilder : ITemperatureBuilder {

	private readonly IGeometry _geometry;

	public TemperatureBuilder(
		IGeometry geometry
	) {
		_geometry = geometry;
	}

	Dictionary<Cell, int> ITemperatureBuilder.Create(
		Size size,
		Voronoi fineVoronoi,
		HashSet<Cell> fineLandforms,
		HashSet<Cell> mountains,
		HashSet<Cell> hills,
		HashSet<Cell> oceans,
		HashSet<Cell> lakes
	) {
		int midLine = size.Rows / 2;

		Dictionary<Cell, int> result = new Dictionary<Cell, int>();
		foreach( Cell cell in fineVoronoi.Cells ) {
			result[cell] = 0;
		}

		int maxValue = 0;
		List<Cell> current = RenderEquator( midLine, size, fineVoronoi, result );
		do {
			current = HeatNeighbours( fineVoronoi, current, result, ref maxValue );
		} while( current.Count > 0 );

		foreach( KeyValuePair<Cell, int> entry in result ) {
			result[entry.Key] = maxValue - entry.Value;
		}

		foreach( Cell land in fineLandforms ) {
			result[land] = result[land] + (maxValue / 5);
		}

		foreach( Cell mountain in mountains ) {
			result[mountain] = result[mountain] / 5;
		}

		foreach( Cell hill in hills ) {
			result[hill] = result[hill] / 3;
		}

		return result;
	}

	private List<Cell> RenderEquator(
		int midLine,
		Size size,
		Voronoi voronoi,
		Dictionary<Cell, int> temperatureMap
	) {
		List<Cell> result = new List<Cell>();
		_geometry.RasterizeLine(
			new Point( 0, midLine ),
			new Point( size.Columns, midLine ),
			( int x, int y ) => {
				Cell? cell = FindCell( voronoi, x, y );
				if( cell is not null
					&& temperatureMap[cell] == 0
				) {
					temperatureMap[cell] = 1;
					result.Add( cell );
				}
			}
		);
		return result;
	}

	private static List<Cell> HeatNeighbours(
		Voronoi fineVoronoi,
		List<Cell> current,
		Dictionary<Cell, int> result,
		ref int maxValue
	) {
		List<Cell> neighbours = new List<Cell>();
		foreach( Cell source in current ) {
			foreach( Cell neighbour in fineVoronoi.Neighbours[source].Where( c => !c.IsOpen ) ) {
				if( result[neighbour] == 0 ) {
					if( neighbour.Center.Y < source.Center.Y
						|| neighbour.Center.Y > source.Center.Y
					) {
						result[neighbour] = result[source] + 1;

						if( result[neighbour] > maxValue ) {
							maxValue = result[neighbour];
						}

					} else {
						result[neighbour] = result[source];
					}
					neighbours.Add( neighbour );
				}
			}
		}
		return neighbours;
	}

	private Cell? FindCell(
		Voronoi voronoi,
		int x,
		int y
	) {
		foreach( Cell cell in voronoi.Cells ) {
			if( _geometry.PointInPolygon( cell.Points, x, y ) ) {
				return cell;
			}
		}
		return null;
	}
}
