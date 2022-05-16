using Common.Geometry.DelaunayVoronoi;

namespace Common.Worlds.Builder;

internal interface IFineLandformBuilder {
	Voronoi Create(
		Size size,
		int pointCount,
		List<Cell> roughLandforms,
		out List<Cell> fineLandforms
	);
}

