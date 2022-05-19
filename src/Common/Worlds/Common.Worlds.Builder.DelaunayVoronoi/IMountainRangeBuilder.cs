using Common.Geometry.DelaunayVoronoi;

namespace Common.Worlds.Builder.DelaunayVoronoi;

internal interface IMountainRangeBuilder {

	HashSet<Cell> BuildRanges(
		Size size,
		Voronoi fineVoronoi,
		HashSet<Cell> fineLandforms
	);
}
