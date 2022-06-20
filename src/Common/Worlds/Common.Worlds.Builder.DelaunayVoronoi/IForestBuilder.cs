namespace Common.Worlds.Builder.DelaunayVoronoi;

internal interface IForestBuilder {

	public HashSet<Cell> Create(
		Voronoi fineVoronoi,
		HashSet<Cell> fineLandforms,
		HashSet<Cell> mountains,
		HashSet<Cell> hills,
		Dictionary<Cell, float> temperatures,
		Dictionary<Cell, float> moistures
	);
}
