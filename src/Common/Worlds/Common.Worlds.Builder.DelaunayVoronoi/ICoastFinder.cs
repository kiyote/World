namespace Common.Worlds.Builder.DelaunayVoronoi;

public interface ICoastFinder {

	IReadOnlySet<Cell> Find(
		ISize size,
		ISearchableVoronoi map,
		IReadOnlySet<Cell> landform,
		IReadOnlySet<Cell> saltwater
	);
}
