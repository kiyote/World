namespace Common.Worlds.Builder;

internal class HexNeighbourLocator : INeighbourLocator {
	IEnumerable<(int column, int row)> INeighbourLocator.GetNeighbours(
		int columns,
		int rows,
		int column,
		int row
	) {
		List<(int column, int row)> result;

		if( ( column & 1 ) == 0 ) {
			result = new List<(int column, int row)>() {
				new() { column = column + 1, row = row },
				new() { column = column, row = row + 1 },
				new() { column = column - 1, row = row },

				new() { column = column - 1, row = row - 1 },
				new() { column = column, row = row - 1 },
				new() { column = column + 1, row = row - 1 },
			};
		} else {
			result = new List<(int column, int row)>() {
				new() { column = column + 1, row = row + 1 },
				new() { column = column, row = row + 1 },
				new() { column = column - 1, row = row + 1 },

				new() { column = column - 1, row = row },
				new() { column = column, row = row - 1 },
				new() { column = column + 1, row = row },
			};
		}

		return result
			.Where(
				p => p.column >= 0
				&& p.column < columns
				&& p.row >= 0
				&& p.row < rows
			);
	}
}

