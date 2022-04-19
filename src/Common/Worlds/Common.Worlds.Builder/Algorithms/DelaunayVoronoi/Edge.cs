namespace Common.Worlds.Builder.Algorithms.DelaunayVoronoi;

public sealed record Edge(
	DelaunayCell From,
	DelaunayCell To
);
