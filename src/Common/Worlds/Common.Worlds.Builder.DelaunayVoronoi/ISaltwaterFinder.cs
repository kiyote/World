namespace Common.Worlds.Builder.DelaunayVoronoi;

public interface ISaltwaterFinder {
	IReadOnlySet<Cell> Find(
		ISize size,
		ISearchableVoronoi map,
		IReadOnlySet<Cell> landform
	);
}
