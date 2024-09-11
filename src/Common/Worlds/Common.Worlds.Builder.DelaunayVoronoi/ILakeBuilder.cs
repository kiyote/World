namespace Common.Worlds.Builder.DelaunayVoronoi;

public interface ILakeBuilder {

	IReadOnlyList<IReadOnlySet<Cell>> Create(
		ISize size,
		IVoronoi map,
		IReadOnlySet<Cell> landform,
		IReadOnlySet<Cell> saltwater,
		IReadOnlySet<Cell> freshwater
	);
}
