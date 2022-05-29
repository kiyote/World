namespace Common.Worlds.Builder.DelaunayVoronoi;

internal sealed class MoistureBuilder : IMoistureBuilder {

	Dictionary<Cell, int> IMoistureBuilder.Create(
		Voronoi fineVoronoi,
		HashSet<Cell> fineLandforms,
		HashSet<Cell> mountains,
		HashSet<Cell> hills,
		HashSet<Cell> saltwater,
		HashSet<Cell> freshwater
	) {
		throw new NotImplementedException();
	}
}
