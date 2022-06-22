namespace Common.Worlds.Builder.DelaunayVoronoi;

internal interface ISaltwaterBuilder {
	HashSet<Cell> Create(
		Size size,
		IVoronoi fineVoronoi,
		HashSet<Cell> fineLandforms
	);
}
