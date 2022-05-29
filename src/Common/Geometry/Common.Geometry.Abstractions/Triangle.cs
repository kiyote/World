namespace Common.Geometry;

public sealed class Triangle {

	public Triangle(
		Point p1,
		Point p2,
		Point p3
	) {
		P1 = p1;
		P2 = p2;
		P3 = p3;
	}

	public Point P1 { get; init; }

	public Point P2 { get; init; }

	public Point P3 { get; init; }
}
