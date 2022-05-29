namespace Common.Worlds.Builder.DelaunayVoronoi;

internal interface IMoistureBuilder {

	Dictionary<Cell, int> Create(
		Voronoi fineVoronoi,
		HashSet<Cell> fineLandforms,
		HashSet<Cell> mountains,
		HashSet<Cell> hills,
		HashSet<Cell> saltwater,
		HashSet<Cell> freshwater
	);
}
