namespace Common.Worlds.Builder.DelaunayVoronoi;

public interface IInlandDistanceBuilder {

	IReadOnlyDictionary<Cell, float> Create(
		ISize size,
		ISearchableVoronoi map,
		IReadOnlySet<Cell> landform,
		IReadOnlySet<Cell> coast
	);
}
