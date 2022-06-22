namespace Common.Geometry.DelaunayVoronoi;

public sealed record Cell(
	Point Center,
	IReadOnlyList<Point> Points,
	bool IsOpen,
	IRect BoundingBox
);
