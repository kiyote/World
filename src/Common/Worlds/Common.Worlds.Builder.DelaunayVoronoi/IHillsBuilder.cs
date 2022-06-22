namespace Common.Worlds.Builder.DelaunayVoronoi;

internal interface IHillsBuilder {

	public HashSet<Cell> Create(
		IVoronoi fineVoronoi,
		HashSet<Cell> fineLandforms,
		HashSet<Cell> mountains
	);
}
