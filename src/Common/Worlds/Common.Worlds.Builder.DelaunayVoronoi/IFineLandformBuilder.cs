using Common.Geometry.DelaunayVoronoi;

namespace Common.Worlds.Builder.DelaunayVoronoi;

internal interface IFineLandformBuilder {
	Voronoi Create(
		Size size,
		int pointCount,
		List<Cell> roughLandforms,
		out HashSet<Cell> fineLandforms
	);
}

