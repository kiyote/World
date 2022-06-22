using Common.Geometry.QuadTrees;

namespace Common.Geometry.DelaunayVoronoi;

internal sealed class QuadTreeVoronoiCellLocator : IVoronoiCellLocator {

	private readonly IQuadTree<IRect> _quadTree;
	private readonly Dictionary<IRect, Cell> _bounds;

	public QuadTreeVoronoiCellLocator(
		IQuadTree<IRect> quadTree,
		Voronoi voronoi
	) {
		_quadTree = quadTree;
		_bounds = new Dictionary<IRect, Cell>();
		foreach (Cell cell in voronoi.Cells) {
			_quadTree.Insert( cell.BoundingBox );
			_bounds[cell.BoundingBox] = cell;
		}
	}

	IReadOnlyList<Cell> IVoronoiCellLocator.Locate(
		IRect bounds
	) {
		IReadOnlyList<IRect> result = _quadTree.Query( bounds );
		if (result.Any()) {
			return result.Select( r => _bounds[r] ).ToList();
		}

		return Array.Empty<Cell>();
	}
}
