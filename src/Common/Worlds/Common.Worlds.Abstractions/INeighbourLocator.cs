namespace Common.Worlds;

public interface INeighbourLocator {
	IEnumerable<(int column, int row)> GetNeighbours(
		int columns,
		int rows,
		int column,
		int row
	);
}

