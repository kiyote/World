namespace Common.Worlds.Builder.Algorithms.Delaunay;

internal sealed record DistancePoint(int X, int Y, float Distance) : Point( X, Y ), IComparable<DistancePoint> {

	public DistancePoint(
		Point Point,
		float Distance
	) : this( Point.X, Point.Y, Distance ) {
	}

	int IComparable<DistancePoint>.CompareTo(
		DistancePoint? other
	) {
		if (Distance < other!.Distance) {
			return -1;
		}

		if (Distance > other!.Distance) {
			return 1;
		}

		return 0;
	}
}
