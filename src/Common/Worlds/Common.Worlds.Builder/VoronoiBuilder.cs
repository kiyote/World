using Common.Geometry;
using Common.Geometry.DelaunayVoronoi;

namespace Common.Worlds.Builder;

internal sealed class VoronoiBuilder : IVoronoiBuilder {

	private readonly IRandom _random;
	private readonly IDelaunatorFactory _delaunatorFactory;
	private readonly IVoronoiFactory _voronoiFactory;

	public VoronoiBuilder(
		IRandom random,
		IDelaunatorFactory delaunatorFactory,
		IVoronoiFactory voronoiFactory
	) {
		_random = random;
		_delaunatorFactory = delaunatorFactory;
		_voronoiFactory = voronoiFactory;
	}

	Voronoi IVoronoiBuilder.Create(
		Size size,
		int pointCount
	) {
		List<Point> points = new List<Point>();
		while( points.Count < pointCount ) {
			Point newPoint = new Point(
				_random.NextInt( size.Columns ),
				_random.NextInt( size.Rows )
			);
			if( !points.Any( p => Math.Abs( p.X - newPoint.X ) <= 1 && Math.Abs( p.Y - newPoint.Y ) <= 1 ) ) {
				points.Add( newPoint );
			};
		}
		Delaunator delaunator = _delaunatorFactory.Create( points );
		Voronoi voronoi = _voronoiFactory.Create( delaunator, size.Columns, size.Rows );

		return voronoi;
	}
}
