namespace Common.Core;

public interface INeighbourLocator {
	IEnumerable<Location> GetNeighbours(
		Size size,
		Location location
	);

	IEnumerable<Location> GetNeighbours(
		Size size,
		int column,
		int row
	);

	IEnumerable<Location> GetNeighbours(
		int columns,
		int rows,
		int column,
		int row
	);
}
