namespace Common.Worlds.Builder.DelaunayVoronoi;

internal interface ITemperatureBuilder {
	Dictionary<Cell, float> Create(
		Size size,
		ISearchableVoronoi voronoi,
		HashSet<Cell> fineLandforms,
		HashSet<Cell> mountains,
		HashSet<Cell> hills,
		HashSet<Cell> oceans,
		HashSet<Cell> lakes
	);
}
