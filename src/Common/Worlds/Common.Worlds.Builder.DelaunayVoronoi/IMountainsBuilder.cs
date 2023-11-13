namespace Common.Worlds.Builder.DelaunayVoronoi;

internal interface IMountainsBuilder {

	HashSet<Cell> Create(
		ISize size,
		ISearchableVoronoi voronoi,
		HashSet<Cell> fineLandforms
	);
}
