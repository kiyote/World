using Common.Geometry.QuadTrees;

namespace Common.Geometry.DelaunayVoronoi;

internal sealed class QuadTreeSearchableVoronoiFactory : ISearchableVoronoiFactory {

	private readonly IQuadTreeFactory _quadTreeFactory;

	public QuadTreeSearchableVoronoiFactory(
		IQuadTreeFactory quadTreeFactory
	) {
		_quadTreeFactory = quadTreeFactory;
	}

	ISearchableVoronoi ISearchableVoronoiFactory.Create(
		IVoronoi voronoi,
		Size size
	) {
		return new QuadTreeSearchableVoronoi(
			_quadTreeFactory.Create<IRect>( new Rect( 0, 0, size.Columns, size.Rows ) ),
			voronoi
		);
	}
}
