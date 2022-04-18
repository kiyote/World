namespace Common.Worlds.Builder.Algorithms.DelaunayVoronoi;

public sealed record Region(
	IReadOnlyList<Cell> Cells,
	IReadOnlyList<Edge> Edges
);
