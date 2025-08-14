namespace Common.Worlds.Builder.DelaunayVoronoi;

public interface IFreshwaterFinder {
	IReadOnlySet<Cell> Create(
		ISize size,
		IVoronoi map,
		IReadOnlySet<Cell> landform,
		IReadOnlySet<Cell> saltwater
	);
}
