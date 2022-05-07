namespace Common.Worlds.Builder;

internal class HexNeighbourLocator : INeighbourLocator {
	IEnumerable<Location> INeighbourLocator.GetNeighbours(
		Size size,
		Location location
	) {
		return DoGetNeighbours( size.Columns, size.Rows, location.Column, location.Row );
	}

	IEnumerable<Location> INeighbourLocator.GetNeighbours(
		Size size,
		int column,
		int row
	) {
		return DoGetNeighbours( size.Columns, size.Rows, column, row );
	}

	IEnumerable<Location> INeighbourLocator.GetNeighbours(
		int columns,
		int rows,
		int column,
		int row
	) {
		return DoGetNeighbours( columns, rows, column, row );
	}

	private static IEnumerable<Location> DoGetNeighbours(
		int columns,
		int rows,
		int column,
		int row
	) {
		List<Location> result;

		if( ( column & 1 ) == 0 ) {
			result = new List<Location>() {
				new Location( column + 1, row ),
				new Location( column, row + 1 ),
				new Location( column - 1, row ),

				new Location( column - 1, row - 1 ),
				new Location( column, row - 1 ),
				new Location( column + 1, row - 1 ),
			};
		} else {
			result = new List<Location>() {
				new Location( column + 1, row + 1 ),
				new Location( column, row + 1 ),
				new Location( column - 1, row + 1 ),

				new Location( column - 1, row ),
				new Location( column, row - 1 ),
				new Location( column + 1, row ),
			};
		}

		return result
			.Where(
				p => p.Column >= 0
				&& p.Column < columns
				&& p.Row >= 0
				&& p.Row < rows
			);
	}

}

