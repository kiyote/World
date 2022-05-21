using Common.Geometry.DelaunayVoronoi;

namespace Common.Worlds.Builder.DelaunayVoronoi;

internal interface ILandformBuilder {
	HashSet<Cell> Create(
		Size size,
		out Voronoi voronoi
	);
}
