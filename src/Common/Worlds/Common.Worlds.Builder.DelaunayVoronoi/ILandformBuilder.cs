namespace Common.Worlds.Builder.DelaunayVoronoi;

public interface ILandformBuilder {
	IReadOnlySet<Cell> Create(
		ISize size,
		TectonicPlates tectonicPlates,
		out ISearchableVoronoi map
	);
}
