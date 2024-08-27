namespace Common.Worlds.Builder.DelaunayVoronoi;

internal interface IRoughLandformBuilder {

	HashSet<Cell> Create(
		ISize size
	);

}
