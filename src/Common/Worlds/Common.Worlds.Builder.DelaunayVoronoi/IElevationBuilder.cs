namespace Common.Worlds.Builder.DelaunayVoronoi;

internal interface IElevationBuilder {

	Dictionary<Cell, float> Create(
		ISize size,
		ISearchableVoronoi map,
		HashSet<Cell> landform,
		HashSet<Cell> saltwater,
		HashSet<Cell> freshwater
	);
}
