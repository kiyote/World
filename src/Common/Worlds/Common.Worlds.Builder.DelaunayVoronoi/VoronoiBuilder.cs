using Common.Geometry;

namespace Common.Worlds.Builder.DelaunayVoronoi;

internal sealed class VoronoiBuilder : IVoronoiBuilder {

	private readonly IDelaunatorFactory _delaunatorFactory;
	private readonly IVoronoiFactory _voronoiFactory;
	private readonly IPointFactory _pointFactory;

	public VoronoiBuilder(
		IDelaunatorFactory delaunatorFactory,
		IVoronoiFactory voronoiFactory,
		IPointFactory pointFactory
	) {
		_delaunatorFactory = delaunatorFactory;
		_voronoiFactory = voronoiFactory;
		_pointFactory = pointFactory;
	}

	Voronoi IVoronoiBuilder.Create(
		Size size,
		int pointCount
	) {
		IReadOnlyList<IPoint> points = _pointFactory.Random( pointCount, size, 1 );
		Delaunator delaunator = _delaunatorFactory.Create( points );
		Voronoi voronoi = _voronoiFactory.Create( delaunator, size.Columns, size.Rows );

		return voronoi;
	}
}
