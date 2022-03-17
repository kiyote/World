using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

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
		List<(int column, int row)> expected = new List<(int column, int row)>() {
			new() { column = 0, row = 1 },
			new() { column = 1, row = 0 },
			new() { column = 2, row = 1 },
			new() { column = 0, row = 2 },
			new() { column = 1, row = 2 },
			new() { column = 2, row = 2 },
		};

		IEnumerable<(int column, int row)> neighbours = _neighbourLocator.GetNeighbours( 3, 3, 1, 1 );
		CollectionAssert.AreEquivalent( expected, neighbours );
	}

	[Test]
	public void GetNeighbours_EvenColumn_6Neighbours() {
		List<(int column, int row)> expected = new List<(int column, int row)>() {
			new() { column = 1, row = 0 },
			new() { column = 2, row = 0 },
			new() { column = 3, row = 0 },
			new() { column = 3, row = 1 },
			new() { column = 2, row = 2 },
			new() { column = 1, row = 1 },
		};

		IEnumerable<(int column, int row)> neighbours = _neighbourLocator.GetNeighbours( 4, 3, 2, 1 );
		CollectionAssert.AreEquivalent( expected, neighbours );
	}

	[Test]
	public void GetNeighbours_TopLeft_2Neighbours() {
		List<(int column, int row)> expected = new List<(int column, int row)>() {
			new() { column = 1, row = 0 },
			new() { column = 0, row = 1 },
		};

		IEnumerable<(int column, int row)> neighbours = _neighbourLocator.GetNeighbours( 2, 2, 0, 0 );
		CollectionAssert.AreEquivalent( expected, neighbours );
	}

	[Test]
	public void GetNeighbours_OddBottom_3Neighbours() {
		List<(int column, int row)> expected = new List<(int column, int row)>() {
			new() { column = 0, row = 2 },
			new() { column = 1, row = 1 },
			new() { column = 2, row = 2 },
		};

		IEnumerable<(int column, int row)> neighbours = _neighbourLocator.GetNeighbours( 3, 3, 1, 2 );
		CollectionAssert.AreEquivalent( expected, neighbours );
	}

	[Test]
	public void GetNeighbours_EvenBottom_5Neighbours() {
		List<(int column, int row)> expected = new List<(int column, int row)>() {
			new() { column = 1, row = 2 },
			new() { column = 1, row = 1 },
			new() { column = 2, row = 1 },
			new() { column = 3, row = 1 },
			new() { column = 3, row = 2 },
		};

		IEnumerable<(int column, int row)> neighbours = _neighbourLocator.GetNeighbours( 4, 3, 2, 2 );
		CollectionAssert.AreEquivalent( expected, neighbours );
	}

	[Test]
	public void GetNeighbours_EvenBottomRight_3Neighbours() {
		List<(int column, int row)> expected = new List<(int column, int row)>() {
			new() { column = 1, row = 2 },
			new() { column = 1, row = 1 },
			new() { column = 2, row = 1 },
		};

		IEnumerable<(int column, int row)> neighbours = _neighbourLocator.GetNeighbours( 3, 3, 2, 2 );
		CollectionAssert.AreEquivalent( expected, neighbours );
	}
}
