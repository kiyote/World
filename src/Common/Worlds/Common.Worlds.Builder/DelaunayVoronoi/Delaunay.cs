namespace Common.Worlds.Builder.DelaunayVoronoi;

public sealed record Delaunay(
	IReadOnlyList<Point> Points,
	IReadOnlyList<Edge> Edges,
	IReadOnlyList<Triangle> Triangles
);
