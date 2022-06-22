using Common.Geometry;

namespace Common.Worlds.Builder.DelaunayVoronoi;

internal sealed class VoronoiBuilder : IVoronoiBuilder {

	private readonly IDelaunatorFactory _delaunatorFactory;
	private readonly IVoronoiFactory _voronoiFactory;
	private readonly IPointFactory _pointFactory;
	private readonly ISearchableVoronoiFactory _searchableVoronoiFactory;

	public VoronoiBuilder(
		IDelaunatorFactory delaunatorFactory,
		IVoronoiFactory voronoiFactory,
		IPointFactory pointFactory,
		ISearchableVoronoiFactory searchableVoronoiFactory
	) {
		_delaunatorFactory = delaunatorFactory;
		_voronoiFactory = voronoiFactory;
		_pointFactory = pointFactory;
		_searchableVoronoiFactory = searchableVoronoiFactory;
	}

	ISearchableVoronoi IVoronoiBuilder.Create(
		Size size,
		int pointCount
	) {
		IReadOnlyList<IPoint> points = _pointFactory.Random( pointCount, size, 5 );
		Delaunator delaunator = _delaunatorFactory.Create( points );
		IVoronoi voronoi = _voronoiFactory.Create( delaunator, size.Columns, size.Rows );
		ISearchableVoronoi searchableVoronoi = _searchableVoronoiFactory.Create( voronoi, size );

		return searchableVoronoi;
	}
}
