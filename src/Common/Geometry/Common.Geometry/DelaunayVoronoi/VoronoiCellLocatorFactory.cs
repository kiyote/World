using Common.Geometry.QuadTrees;

namespace Common.Geometry.DelaunayVoronoi;

internal class VoronoiCellLocatorFactory : IVoronoiCellLocatorFactory {

	private readonly IQuadTreeFactory _quadTreeFactory;

	public VoronoiCellLocatorFactory(
		IQuadTreeFactory quadTreeFactory
	) {
		_quadTreeFactory = quadTreeFactory;
	}

	IVoronoiCellLocator IVoronoiCellLocatorFactory.Create(
		Voronoi voronoi,
		Size size
	) {
		return new QuadTreeVoronoiCellLocator(
			_quadTreeFactory.Create<IRect>( new Rect( 0, 0, size.Columns, size.Rows ) ),
			voronoi
		);
	}
}
