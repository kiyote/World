namespace Common.Worlds.Builder.Algorithms.DelaunayVoronoi;

public sealed record Voronoi(
	IReadOnlyList<Region> Regions
);
