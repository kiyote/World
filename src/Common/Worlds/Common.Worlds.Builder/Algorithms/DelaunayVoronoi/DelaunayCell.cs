namespace Common.Worlds.Builder.Algorithms.DelaunayVoronoi;

public sealed record DelaunayCell(
	Simplex Simplex,
	Vertex CircumCenter,
	float Radius
);
