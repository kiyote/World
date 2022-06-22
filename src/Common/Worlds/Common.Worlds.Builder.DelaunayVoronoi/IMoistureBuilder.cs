namespace Common.Worlds.Builder.DelaunayVoronoi;

internal interface IMoistureBuilder {

	Dictionary<Cell, float> Create(
		Size size,
		ISearchableVoronoi voronoi,
		HashSet<Cell> fineLandforms,
		HashSet<Cell> saltwater,
		HashSet<Cell> freshwater,
		Dictionary<Cell, float> temperature,
		Dictionary<Cell, float> airflow
	);
}
