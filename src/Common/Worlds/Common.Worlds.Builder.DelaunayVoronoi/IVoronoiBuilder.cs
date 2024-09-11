namespace Common.Worlds.Builder.DelaunayVoronoi;

public interface IVoronoiBuilder {

	ISearchableVoronoi Create(
		ISize size,
		int cellSize
	);

}
