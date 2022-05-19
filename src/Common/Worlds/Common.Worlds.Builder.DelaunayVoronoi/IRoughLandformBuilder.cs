using Common.Geometry.DelaunayVoronoi;

namespace Common.Worlds.Builder.DelaunayVoronoi;

internal interface IRoughLandformBuilder {

	Voronoi Create(
		Size size,
		float landPercentage,
		out List<Cell> roughLandforms
	);
}
