namespace Common.Worlds.Builder.DelaunayVoronoi;

internal interface ILandformBuilder {
	HashSet<Cell> Create(
		Size size,
		out ISearchableVoronoi voronoi
	);
}
