namespace Common.Worlds.Builder.DelaunayVoronoi;

internal interface IVoronoiBuilder {

	ISearchableVoronoi Create(
		Size size,
		int pointCount
	);
}
