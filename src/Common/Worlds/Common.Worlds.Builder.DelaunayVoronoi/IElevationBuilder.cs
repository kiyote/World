namespace Common.Worlds.Builder.DelaunayVoronoi;

public interface IElevationBuilder {

	IReadOnlyDictionary<Cell, float> Create(
		ISize size,
		TectonicPlates tectonicPlates,
		ISearchableVoronoi map,
		IReadOnlySet<Cell> landform,
		IReadOnlyDictionary<Cell, float> inlandDistance
	);
}
