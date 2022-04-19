namespace Common.Worlds.Builder.Algorithms.DelaunayVoronoi;

public sealed record VoronoiRegion(
	IReadOnlyList<DelaunayCell> Cells,
	IReadOnlyList<Edge> Edges
);
