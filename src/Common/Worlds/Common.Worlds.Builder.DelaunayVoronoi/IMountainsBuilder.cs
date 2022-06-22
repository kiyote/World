namespace Common.Worlds.Builder.DelaunayVoronoi;

internal interface IMountainsBuilder {

	HashSet<Cell> Create(
		Size size,
		ISearchableVoronoi voronoi,
		HashSet<Cell> fineLandforms
	);
}
