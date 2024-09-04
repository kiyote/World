namespace Common.Worlds.Builder.DelaunayVoronoi;

internal interface ISaltwaterBuilder {
	HashSet<Cell> Create(
		ISize size,
		IVoronoi map,
		HashSet<Cell> landform
	);
}
