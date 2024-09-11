namespace Common.Worlds.Builder.DelaunayVoronoi;

public interface IElevationBuilder {

	IReadOnlyDictionary<Cell, float> Create(
		ISize size,
		ISearchableVoronoi map,
		IReadOnlySet<Cell> landform,
		IReadOnlySet<Cell> saltwater,
		IReadOnlySet<Cell> freshwater,
		IReadOnlyList<IReadOnlySet<Cell>> lakes
	);
}
