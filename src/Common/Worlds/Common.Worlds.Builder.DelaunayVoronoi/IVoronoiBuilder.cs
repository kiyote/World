namespace Common.Worlds.Builder.DelaunayVoronoi;

internal interface IVoronoiBuilder {

	ISearchableVoronoi Create(
		ISize size,
		int cellSize
	);

}
