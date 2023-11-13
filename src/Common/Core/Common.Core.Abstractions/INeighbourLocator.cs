using Kiyote.Geometry;

namespace Common.Core;

public interface INeighbourLocator {
	IEnumerable<Point> GetNeighbours(
		ISize size,
		Point location
	);

	IEnumerable<Point> GetNeighbours(
		ISize size,
		int column,
		int row
	);

	IEnumerable<Point> GetNeighbours(
		int columns,
		int rows,
		int column,
		int row
	);
}
