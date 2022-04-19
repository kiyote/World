﻿namespace Common.Worlds.Builder.Algorithms.ConvexHull;

internal sealed record HullPoint( int X, int Y ) : IPoint {
	public HullPoint(
		IPoint point
	): this( point.X, point.Y ) {
	}

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

