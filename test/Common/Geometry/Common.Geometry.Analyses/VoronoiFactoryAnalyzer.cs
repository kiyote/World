using Common.Core;
using Common.Geometry.DelaunayVoronoi;

namespace Common.Geometry.Analyses;

internal sealed class VoronoiFactoryAnalyzer {

	public static void Analyze(
		IPointFactory pointFactory,
		IDelaunatorFactory delaunatorFactory,
		IVoronoiFactory voronoiFactory
	) {
		int pointCount = 100;
		Size size = new Size( 500, 500 );
		IReadOnlyList<IPoint> points = pointFactory.Random( pointCount, size, 1 );

		Delaunator delaunator = delaunatorFactory.Create( points );

		IVoronoi voronoi = voronoiFactory.Create( delaunator, size.Columns, size.Rows );
	}
}
