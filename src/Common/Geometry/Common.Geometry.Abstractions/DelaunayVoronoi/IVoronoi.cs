namespace Common.Geometry.DelaunayVoronoi;

public interface IVoronoi {

	IReadOnlyList<Cell> Cells { get; }

	IReadOnlyList<Edge> Edges { get; }

	IReadOnlyDictionary<Cell, IReadOnlyList<Cell>> Neighbours { get; }
}
