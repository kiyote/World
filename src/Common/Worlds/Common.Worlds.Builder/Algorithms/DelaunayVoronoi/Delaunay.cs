namespace Common.Worlds.Builder.Algorithms.DelaunayVoronoi;

public sealed record Delaunay(
	IReadOnlyList<Vertex> Vertices,
	IReadOnlyList<Cell> Cells,
	Vertex Centroid
);

