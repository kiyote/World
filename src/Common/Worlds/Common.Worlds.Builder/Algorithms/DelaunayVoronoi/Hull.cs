namespace Common.Worlds.Builder.Algorithms.DelaunayVoronoi;

internal sealed record Hull(
	Vertex Centroid,
	IReadOnlyList<Vertex> Vertices,
	IReadOnlyList<Simplex> Simplexes
);
