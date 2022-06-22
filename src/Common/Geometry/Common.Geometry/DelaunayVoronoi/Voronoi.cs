namespace Common.Geometry.DelaunayVoronoi;

internal sealed class Voronoi : IVoronoi {

	public Voronoi(
		IReadOnlyList<Edge> edges,
		IReadOnlyList<Cell> cells,
		IReadOnlyDictionary<Cell, IReadOnlyList<Cell>> neighbours
	) {
		Edges = edges;
		Cells = cells;
		Neighbours = neighbours;
	}

	public IReadOnlyList<Edge> Edges { get; init; }
	public IReadOnlyList<Cell> Cells { get; init; }
	public IReadOnlyDictionary<Cell, IReadOnlyList<Cell>> Neighbours { get; init; }
}

