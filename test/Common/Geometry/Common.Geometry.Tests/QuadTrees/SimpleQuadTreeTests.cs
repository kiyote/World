namespace Common.Geometry.QuadTrees.Tests;

[TestFixture]
public sealed class SimpleQuadTreeTests {

	private SimpleQuadTree<Rect> _tree;

	[SetUp]
	public void SetUp() {
		Rect bounds = new Rect( 0, 0, 1000, 1000 );
		_tree = new SimpleQuadTree<Rect>( bounds );
	}

	[Test]
	public void Query_OneRectFullyInside_OneRectReturned() {
		Rect area = new Rect( 200, 200, 300, 300 );
		Rect item = new Rect( 210, 210, 290, 290 );
		_tree.Insert(
			item
		);

		IReadOnlyList<Rect> result = _tree.Query( area );

		Assert.AreEqual( 1, result.Count );
		Assert.AreSame( item, result[0] );
	}

	[Test]
	public void Query_OneRectPartiallyInside_OneRectReturned() {
		Rect area = new Rect( 200, 200, 300, 300 );
		Rect item = new Rect( 190, 190, 290, 290 );
		_tree.Insert(
			item
		);

		IReadOnlyList<Rect> result = _tree.Query( area );

		Assert.AreEqual( 1, result.Count );
		Assert.AreSame( item, result[0] );
	}

	[Test]
	public void Query_OneRectFullyOutside_ZeroRectReturned() {
		Rect area = new Rect( 200, 200, 300, 300 );
		Rect item = new Rect( 100, 100, 199, 199 );
		_tree.Insert(
			item
		);

		IReadOnlyList<Rect> result = _tree.Query( area );

		Assert.AreEqual( 0, result.Count );
	}

	[Test]
	public void Query_TwoRectOneInsideOnePartial_TwoRectReturned() {
		Rect area = new Rect( 200, 200, 300, 300 );
		Rect item1 = new Rect( 210, 210, 290, 290 );
		Rect item2 = new Rect( 190, 190, 290, 290 );
		Rect[] items = new Rect[] { item1, item2 };
		_tree.Insert(
			item1
		);
		_tree.Insert(
			item2
		);

		IReadOnlyList<Rect> result = _tree.Query( area );

		Assert.AreEqual( 2, result.Count );
		CollectionAssert.AreEquivalent( items, result );
	}

	[Test]
	public void Query_ThreeRectOneInsideOnePartialOneOutside_TwoRectReturned() {
		Rect area = new Rect( 200, 200, 300, 300 );
		Rect item1 = new Rect( 210, 210, 290, 290 );
		Rect item2 = new Rect( 100, 100, 199, 199 );
		Rect item3 = new Rect( 190, 190, 290, 290 );
		Rect[] items = new Rect[] { item1, item3 };
		_tree.Insert(
			item1
		);
		_tree.Insert(
			item2
		);
		_tree.Insert(
			item3
		);

		IReadOnlyList<Rect> result = _tree.Query( area );

		Assert.AreEqual( 2, result.Count );
		CollectionAssert.AreEquivalent( items, result );
	}
}
