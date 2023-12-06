namespace Common.Worlds.Builder.Tests;

using Point = Kiyote.Geometry.Point;

[TestFixture]
public sealed class HexNeighbourLocatorUnitTests {

	private INeighbourLocator _neighbourLocator;

	[SetUp]
	public void SetUp() {
		_neighbourLocator = new HexNeighbourLocator();
	}

	[Test]
	public void GetNeighbours_OddColumn_6Neighbours() {
		List<Point> expected = [
			new Point( 0, 1 ),
			new Point( 1, 0 ),
			new Point( 2, 1 ),
			new Point( 0, 2 ),
			new Point( 1, 2 ),
			new Point( 2, 2 ),
		];

		IEnumerable<Point> neighbours = _neighbourLocator.GetNeighbours( 3, 3, 1, 1 );
		Assert.That( expected, Is.EquivalentTo( neighbours ) );
	}

	[Test]
	public void GetNeighbours_EvenColumn_6Neighbours() {
		List<Point> expected = [
			new Point( 1, 0 ),
			new Point( 2, 0 ),
			new Point( 3, 0 ),
			new Point( 3, 1 ),
			new Point( 2, 2 ),
			new Point( 1, 1 ),
		];

		IEnumerable<Point> neighbours = _neighbourLocator.GetNeighbours( 4, 3, 2, 1 );
		Assert.That( expected, Is.EquivalentTo( neighbours ) );
	}

	[Test]
	public void GetNeighbours_TopLeft_2Neighbours() {
		List<Point> expected = [
			new Point( 1, 0 ),
			new Point( 0, 1 ),
		];

		IEnumerable<Point> neighbours = _neighbourLocator.GetNeighbours( 2, 2, 0, 0 );
		Assert.That( expected, Is.EquivalentTo( neighbours ) );
	}

	[Test]
	public void GetNeighbours_OddBottom_3Neighbours() {
		List<Point> expected = [
			new Point( 0, 2 ),
			new Point( 1, 1 ),
			new Point( 2, 2 ),
		];

		IEnumerable<Point> neighbours = _neighbourLocator.GetNeighbours( 3, 3, 1, 2 );
		Assert.That( expected, Is.EquivalentTo( neighbours ) );
	}

	[Test]
	public void GetNeighbours_EvenBottom_5Neighbours() {
		List<Point> expected = [
			new Point( 1, 2 ),
			new Point( 1, 1 ),
			new Point( 2, 1 ),
			new Point( 3, 1 ),
			new Point( 3, 2 ),
		];

		IEnumerable<Point> neighbours = _neighbourLocator.GetNeighbours( 4, 3, 2, 2 );
		Assert.That( expected, Is.EquivalentTo( neighbours ) );
	}

	[Test]
	public void GetNeighbours_EvenBottomRight_3Neighbours() {
		List<Point> expected = [
			new Point( 1, 2 ),
			new Point( 1, 1 ),
			new Point( 2, 1 ),
		];

		IEnumerable<Point> neighbours = _neighbourLocator.GetNeighbours( 3, 3, 2, 2 );
		Assert.That( expected, Is.EquivalentTo( neighbours ) );
	}
}
