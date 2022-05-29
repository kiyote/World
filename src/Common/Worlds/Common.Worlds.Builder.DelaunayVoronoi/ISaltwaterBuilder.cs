namespace Common.Worlds.Builder.DelaunayVoronoi;

internal interface ISaltwaterBuilder {
	HashSet<Cell> Create(
		Size size,
		Voronoi fineVoronoi,
		HashSet<Cell> fineLandforms
	);
}
