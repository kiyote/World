namespace Common.Worlds.Builder.DelaunayVoronoi;

internal interface IMountainsBuilder {

	HashSet<Cell> Create(
		Size size,
		Voronoi fineVoronoi,
		IVoronoiCellLocator cellLocator,
		HashSet<Cell> fineLandforms
	);
}
