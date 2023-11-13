namespace Common.Worlds.Builder.DelaunayVoronoi;

internal interface ISaltwaterBuilder {
	HashSet<Cell> Create(
		ISize size,
		IVoronoi fineVoronoi,
		HashSet<Cell> fineLandforms
	);
}
