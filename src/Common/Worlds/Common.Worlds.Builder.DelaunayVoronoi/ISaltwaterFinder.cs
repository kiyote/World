namespace Common.Worlds.Builder.DelaunayVoronoi;

public interface ISaltwaterFinder {
	IReadOnlySet<Cell> Find(
		ISize size,
		IVoronoi map,
		IReadOnlySet<Cell> landform
	);
}
