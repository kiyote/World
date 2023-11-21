namespace Common.Worlds.Builder;

internal class HexNeighbourLocator : INeighbourLocator {
	IEnumerable<Point> INeighbourLocator.GetNeighbours(
		ISize size,
		Point location
	) {
		return DoGetNeighbours( size.Width, size.Height, location.X, location.Y );
	}

	IEnumerable<Point> INeighbourLocator.GetNeighbours(
		ISize size,
		int column,
		int row
	) {
		return DoGetNeighbours( size.Width, size.Height, column, row );
	}

	IEnumerable<Point> INeighbourLocator.GetNeighbours(
		int columns,
		int rows,
		int column,
		int row
	) {
		return DoGetNeighbours( columns, rows, column, row );
	}

	private static IEnumerable<Point> DoGetNeighbours(
		int columns,
		int rows,
		int column,
		int row
	) {
		List<Point> result;

		if( ( column & 1 ) == 0 ) {
			result = [
				new Point( column + 1, row ),
				new Point( column, row + 1 ),
				new Point( column - 1, row ),

				new Point( column - 1, row - 1 ),
				new Point( column, row - 1 ),
				new Point( column + 1, row - 1 ),
			];
		} else {
			result = [
				new Point( column + 1, row + 1 ),
				new Point( column, row + 1 ),
				new Point( column - 1, row + 1 ),

				new Point( column - 1, row ),
				new Point( column, row - 1 ),
				new Point( column + 1, row ),
			];
		}

		return result
			.Where(
				p => p.X >= 0
				&& p.X < columns
				&& p.Y >= 0
				&& p.Y < rows
			);
	}

}

