namespace Common.Worlds.Builder.DelaunayVoronoi;

internal interface ITemperatureBuilder {
	Dictionary<Cell, int> Create(
		Size size,
		Voronoi fineVoronoi,
		HashSet<Cell> fineLandforms,
		HashSet<Cell> mountains,
		HashSet<Cell> hills,
		HashSet<Cell> oceans,
		HashSet<Cell> lakes
	);
}
