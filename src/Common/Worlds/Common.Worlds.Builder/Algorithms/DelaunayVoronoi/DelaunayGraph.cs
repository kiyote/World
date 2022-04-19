namespace Common.Worlds.Builder.Algorithms.DelaunayVoronoi;

public sealed record DelaunayGraph(
	IReadOnlyList<Vertex> Vertices,
	IReadOnlyList<DelaunayCell> Cells,
	Vertex Centroid
);

