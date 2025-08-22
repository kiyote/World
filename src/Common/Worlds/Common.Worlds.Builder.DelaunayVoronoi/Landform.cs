namespace Common.Worlds.Builder.DelaunayVoronoi;

internal class Landform {

	public Landform(
		IReadOnlySet<Cell> cells,
		ISearchableVoronoi map
	) {
		Cells = cells;
		Map = map;
	}

	public IReadOnlySet<Cell> Cells { get; }

	public ISearchableVoronoi Map { get; }
}
