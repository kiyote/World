namespace Common.Geometry.DelaunayVoronoi;

public interface IVoronoiCellLocator {

	IReadOnlyList<Cell> Locate( IRect bounds );
}
