namespace Common.Worlds.Builder.DelaunayVoronoi;

internal interface ILakeBuilder {

	List<HashSet<Cell>> Create(
		ISize size,
		IVoronoi map,
		HashSet<Cell> landform,
		HashSet<Cell> saltwater,
		HashSet<Cell> freshwater
	);
}
