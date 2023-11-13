namespace Common.Worlds.Builder.DelaunayVoronoi;

internal interface ILandformBuilder {
	HashSet<Cell> Create(
		ISize size,
		out ISearchableVoronoi voronoi
	);
}
