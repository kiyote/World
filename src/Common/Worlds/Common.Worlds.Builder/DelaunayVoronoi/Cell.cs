namespace Common.Worlds.Builder.DelaunayVoronoi;

public sealed record Cell(
	Point Circumcenter,
	IReadOnlyList<Point> Points,
	bool IsOpen
);
