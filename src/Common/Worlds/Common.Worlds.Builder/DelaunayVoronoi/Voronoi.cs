namespace Common.Worlds.Builder.DelaunayVoronoi;

public sealed record Voronoi(
	IReadOnlyList<Edge> Edges,
	IReadOnlyList<Cell> Cells,
	IReadOnlyDictionary<Cell, IReadOnlyList<Cell>> Neighbours
);

