namespace Common.Worlds.Builder.Algorithms.DelaunayVoronoi;

public sealed record Cell(
	Simplex Simplex,
	Vertex CircumCenter,
	float Radius
);
