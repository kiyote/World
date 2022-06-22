namespace Common.Worlds.Builder.DelaunayVoronoi;

internal interface IAirflowBuilder {
	Dictionary<Cell, float> Create(
		Size size,
		ISearchableVoronoi voronoi,
		HashSet<Cell> fineLandforms,
		HashSet<Cell> mountains,
		HashSet<Cell> hills
	);
}
