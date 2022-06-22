namespace Common.Worlds.Builder.DelaunayVoronoi;

internal interface IFreshwaterBuilder {
	HashSet<Cell> Create(
		IVoronoi fineVoronoi,
		HashSet<Cell> fineLandforms,
		HashSet<Cell> saltwater
	);
}
