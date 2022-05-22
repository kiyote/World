using Common.Core;

namespace Common.Geometry.Analyses;

internal sealed class PointFactoryAnalyzer {

	public static void Analyze(
		IPointFactory pointFactory
	) {
		int pointCount = 100;
		Size size = new Size( 500, 500 );
		IReadOnlyList<IPoint> points = pointFactory.Random( pointCount, size, 1 );
	}
}
