namespace Common.Worlds.Builder.Tests;

[TestFixture]
public sealed class HexNeighbourLocatorUnitTests {

	private INeighbourLocator _neighbourLocator;

	[SetUp]
	public void SetUp() {
		_neighbourLocator = new HexNeighbourLocator();
	}

	[Test]
	public void GetNeighbours_OddColumn_6Neighbours() {
		List<Location> expected = new List<Location>() {
			new Location( 0, 1 ),
			new Location( 1, 0 ),
			new Location( 2, 1 ),
			new Location( 0, 2 ),
			new Location( 1, 2 ),
			new Location( 2, 2 ),
		};

		IEnumerable<Location> neighbours = _neighbourLocator.GetNeighbours( 3, 3, 1, 1 );
		CollectionAssert.AreEquivalent( expected, neighbours );
	}

	[Test]
	public void GetNeighbours_EvenColumn_6Neighbours() {
		List<Location> expected = new List<Location>() {
			new Location( 1, 0 ),
			new Location( 2, 0 ),
			new Location( 3, 0 ),
			new Location( 3, 1 ),
			new Location( 2, 2 ),
			new Location( 1, 1 ),
		};

		IEnumerable<Location> neighbours = _neighbourLocator.GetNeighbours( 4, 3, 2, 1 );
		CollectionAssert.AreEquivalent( expected, neighbours );
	}

	[Test]
	public void GetNeighbours_TopLeft_2Neighbours() {
		List<Location> expected = new List<Location>() {
			new Location( 1, 0 ),
			new Location( 0, 1 ),
		};

		IEnumerable<Location> neighbours = _neighbourLocator.GetNeighbours( 2, 2, 0, 0 );
		CollectionAssert.AreEquivalent( expected, neighbours );
	}

	[Test]
	public void GetNeighbours_OddBottom_3Neighbours() {
		List<Location> expected = new List<Location>() {
			new Location( 0, 2 ),
			new Location( 1, 1 ),
			new Location( 2, 2 ),
		};

		IEnumerable<Location> neighbours = _neighbourLocator.GetNeighbours( 3, 3, 1, 2 );
		CollectionAssert.AreEquivalent( expected, neighbours );
	}

	[Test]
	public void GetNeighbours_EvenBottom_5Neighbours() {
		List<Location> expected = new List<Location>() {
			new Location( 1, 2 ),
			new Location( 1, 1 ),
			new Location( 2, 1 ),
			new Location( 3, 1 ),
			new Location( 3, 2 ),
		};

		IEnumerable<Location> neighbours = _neighbourLocator.GetNeighbours( 4, 3, 2, 2 );
		CollectionAssert.AreEquivalent( expected, neighbours );
	}

	[Test]
	public void GetNeighbours_EvenBottomRight_3Neighbours() {
		List<Location> expected = new List<Location>() {
			new Location( 1, 2 ),
			new Location( 1, 1 ),
			new Location( 2, 1 ),
		};

		IEnumerable<Location> neighbours = _neighbourLocator.GetNeighbours( 3, 3, 2, 2 );
		CollectionAssert.AreEquivalent( expected, neighbours );
	}
}
