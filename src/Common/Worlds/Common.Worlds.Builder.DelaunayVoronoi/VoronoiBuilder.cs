namespace Common.Worlds.Builder.DelaunayVoronoi;

internal sealed class VoronoiBuilder : IVoronoiBuilder {

	private readonly IVoronoiFactory _voronoiFactory;
	private readonly IPointFactory _pointFactory;
	private readonly ISearchableVoronoiFactory _searchableVoronoiFactory;

	public VoronoiBuilder(
		IVoronoiFactory voronoiFactory,
		IPointFactory pointFactory,
		ISearchableVoronoiFactory searchableVoronoiFactory
	) {
		_voronoiFactory = voronoiFactory;
		_pointFactory = pointFactory;
		_searchableVoronoiFactory = searchableVoronoiFactory;
	}

	ISearchableVoronoi IVoronoiBuilder.Create(
		ISize size,
		int cellSize
	) {
		IReadOnlyList<Point> points = _pointFactory.Fill( size, cellSize );
		IVoronoi voronoi = _voronoiFactory.Create( new Rect( 0, 0, size.Width, size.Height ), points, false );
		ISearchableVoronoi searchableVoronoi = _searchableVoronoiFactory.Create( voronoi, size );

		return searchableVoronoi;
	}
}
