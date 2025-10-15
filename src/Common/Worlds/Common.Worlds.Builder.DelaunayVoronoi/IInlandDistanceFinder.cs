namespace Common.Worlds.Builder.DelaunayVoronoi;

public interface IInlandDistanceFinder {

	Task<IReadOnlyDictionary<Cell, float>> CreateAsync(
		ISize size,
		ISearchableVoronoi map,
		IReadOnlySet<Cell> landform,
		IReadOnlySet<Cell> coast,
		CancellationToken cancellationToken
	);
}
