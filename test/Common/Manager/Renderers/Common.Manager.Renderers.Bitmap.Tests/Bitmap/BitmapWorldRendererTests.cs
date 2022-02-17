using NUnit.Framework;

namespace Common.Manager.Renderers.Bitmap.Tests;

[TestFixture]
public class BitmapWorldRendererTests {

	private BitmapWorldRenderer _renderer;

	[SetUp]
	public void SetUp() {
		_renderer = new BitmapWorldRenderer();
	}

	[Test]
	public void Test1() {
		Assert.Pass();
	}
}
