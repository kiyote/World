namespace Common.Worlds.Builder.Algorithms.Delaunay;

internal sealed record Circumcircle(
	Point Center,
	float Radius,
	IEnumerable<Point> Points
) : ICircle, IComparable<Circumcircle> {

	IPoint ICircle.Center => Center;

	int IComparable<Circumcircle>.CompareTo(
		Circumcircle? other
	) {
		if (Radius < other!.Radius) {
			return -1;
		}

		if (Radius > other!.Radius) {
			return 1;
		}

		return 0;
	}
}
