namespace Common.Worlds.Builder.DelaunayVoronoi;

public interface IFreshwaterBuilder {
	IReadOnlySet<Cell> Create(
		ISize size,
		IVoronoi map,
		IReadOnlySet<Cell> landform,
		IReadOnlySet<Cell> saltwater
	);
}
