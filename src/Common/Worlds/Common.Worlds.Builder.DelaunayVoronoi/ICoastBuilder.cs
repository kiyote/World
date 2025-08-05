namespace Common.Worlds.Builder.DelaunayVoronoi;

public interface ICoastBuilder {

	IReadOnlySet<Cell> Create(
		ISize size,
		ISearchableVoronoi map,
		IReadOnlySet<Cell> landform,
		IReadOnlySet<Cell> saltwater
	);
}
