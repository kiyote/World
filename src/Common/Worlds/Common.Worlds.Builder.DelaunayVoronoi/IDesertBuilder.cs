
namespace Common.Worlds.Builder.DelaunayVoronoi;

internal interface IDesertBuilder {

	public HashSet<Cell> Create(
		HashSet<Cell> fineLandforms,
		HashSet<Cell> mountains,
		HashSet<Cell> hills,
		Dictionary<Cell, float> moistures
	);
}

