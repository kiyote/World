namespace Common.Worlds.Builder.DelaunayVoronoi;

internal interface IFreshwaterBuilder {
	HashSet<Cell> Create(
		ISize size,
		IVoronoi map,
		HashSet<Cell> landform,
		HashSet<Cell> saltwater
	);
}
