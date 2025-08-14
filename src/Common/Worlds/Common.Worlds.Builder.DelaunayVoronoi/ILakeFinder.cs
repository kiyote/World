namespace Common.Worlds.Builder.DelaunayVoronoi;

public interface ILakeFinder {

	IReadOnlyList<IReadOnlySet<Cell>> Finder(
		ISize size,
		IVoronoi map,
		IReadOnlySet<Cell> landform,
		IReadOnlySet<Cell> saltwater,
		IReadOnlySet<Cell> freshwater
	);
}
