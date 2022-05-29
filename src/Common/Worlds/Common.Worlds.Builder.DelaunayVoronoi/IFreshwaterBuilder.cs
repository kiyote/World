namespace Common.Worlds.Builder.DelaunayVoronoi;

internal interface IFreshwaterBuilder {
	HashSet<Cell> Create(
		Voronoi fineVoronoi,
		HashSet<Cell> fineLandforms,
		HashSet<Cell> saltwater
	);
}
