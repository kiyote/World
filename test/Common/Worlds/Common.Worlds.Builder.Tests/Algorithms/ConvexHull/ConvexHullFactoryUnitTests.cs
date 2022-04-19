using NUnit.Framework;

namespace Common.Worlds.Builder.Algorithms.ConvexHull.Tests;

[TestFixture]
public sealed class ConvexHullFactoryUnitTests {

	private IConvexHullFactory _factory;

	[SetUp]
	public void SetUp() {
		_factory = new ConvexHullFactory();
	}

	[Test]
	public void Create_ThreePoints_PointsAreHull() {
		List<HullPoint> points = new List<HullPoint>() {
			new HullPoint( 0.5f, 0.0f ),
			new HullPoint( 0.0f, 1.0f ),
			new HullPoint( 1.0f, 1.0f )
		};
		IReadOnlyList<IPoint> actual = _factory.Create( points );

		CollectionAssert.AreEquivalent( points, actual );
	}

	[Test]
	public void Create_SixPointsWithMiddle_MiddleExcluded() {
		List<HullPoint> points = new List<HullPoint>() {
			new HullPoint( 0.25f, 0.0f ),
			new HullPoint( 0.75f, 0.0f ),

			new HullPoint( 0.0f, 0.5f ),
			new HullPoint( 1.0f, 0.5f ),

			new HullPoint( 0.25f, 1.0f ),
			new HullPoint( 0.75f, 1.0f ),

			new HullPoint( 0.25f, 0.5f ),
			new HullPoint( 0.75f, 0.5f )
		};
		IReadOnlyList<IPoint> actual = _factory.Create( points );

		CollectionAssert.AreEquivalent( points.Take(6), actual );
	}
}
