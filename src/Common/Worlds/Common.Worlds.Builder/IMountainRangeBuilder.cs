using Common.Geometry.DelaunayVoronoi;

namespace Common.Worlds.Builder;

internal interface IMountainRangeBuilder {

	List<Cell> BuildRanges(
		Size size,
		Voronoi fineVoronoi,
		List<Cell> fineLandforms
	);
}
