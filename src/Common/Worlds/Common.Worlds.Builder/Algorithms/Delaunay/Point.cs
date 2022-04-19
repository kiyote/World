namespace Common.Worlds.Builder.Algorithms.Delaunay;

internal record Point( int X, int Y ) : IPoint {

	bool IEquatable<IPoint>.Equals(
		IPoint? other
	) {
		if (other is null) {
			return false;
		}

		if (X == other.X
			&& Y == other.Y
		) {
			return true;
		}

		return false;
	}
}

