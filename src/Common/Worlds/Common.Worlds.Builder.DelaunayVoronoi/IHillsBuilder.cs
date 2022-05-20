using Common.Geometry.DelaunayVoronoi;

namespace Common.Worlds.Builder.DelaunayVoronoi;

internal interface IHillsBuilder {

	public HashSet<Cell> Create(
		Voronoi fineVoronoi,
		HashSet<Cell> fineLandforms,
		HashSet<Cell> mountains
	);
}
