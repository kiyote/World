using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Common.Worlds.Builder.Algorithms.ConvexHull.Tests;

[TestFixture]
public sealed class QuickHullFactoryIntegrationTests {

	private string _folder;
	private IRandom _random;
	private IServiceScope _scope;
	private IServiceProvider _provider;
	private IConvexHullFactory _factory;

	[OneTimeSetUp]
	public void OneTimeSetUp() {
		string testRoot = Path.Combine( Path.GetTempPath(), "world" );
		Directory.CreateDirectory( testRoot );
		_folder = Path.Combine( testRoot, Guid.NewGuid().ToString( "N" ) );
		Directory.CreateDirectory( _folder );

		var services = new ServiceCollection();
		services.AddCore();

		_provider = services.BuildServiceProvider();
	}

	[OneTimeTearDown]
	public void OneTimeTearDown() {
		Directory.Delete( _folder, true );
	}

	[SetUp]
	public void SetUp() {
		_scope = _provider.CreateScope();

		_random = _scope.ServiceProvider.GetRequiredService<IRandom>();

		_factory = new QuickHullFactory();
	}

	[TearDown]
	public void TearDown() {
		_scope.Dispose();
		_scope = null;
	}

	[Test]
	public void Create_ThreePoints_HullMatchesPoints() {
		// Generate a set of points in space
		List<HullPoint> points = new List<HullPoint>();
		for( int i = 0; i < 3; i++ ) {
			points.Add(
				new HullPoint(
					_random.NextInt( 100 ),
					_random.NextInt( 100 )
				)
			);
		}

		IReadOnlyList<IEdge> hull = _factory.Create( points );
		Assert.AreEqual( 3, hull.Count, "Three points should make a 3-edged hull." );
		foreach (IEdge edge in hull) {
			CollectionAssert.Contains( points, edge.First, "First in hull edge not in source points." );
			CollectionAssert.Contains( points, edge.Second, "Second in hull edge not in source points." );
		}
	}

	[Test]
	[Ignore("Used to visualize output.")]
	public void Visualize() {
		// Generate a set of points in space
		List<HullPoint> points = new List<HullPoint>();
		for (int i = 0; i < 100; i++) {
			points.Add(
				new HullPoint(
					_random.NextInt( 1000 ),
					_random.NextInt( 1000 )
				)
			);
		}

		using Image<Rgba32> image = new Image<Rgba32>( 1000, 1000 );

		IReadOnlyList<IEdge> hull = _factory.Create( points );

		PointF[] ends = new PointF[2];
		foreach (IEdge edge in hull) {
			ends[0].X = edge.First.X;
			ends[0].Y = edge.First.Y;
			ends[1].X = edge.Second.X;
			ends[1].Y = edge.Second.Y;
			image.Mutate( i => i.DrawLines( Color.Red, 1.0f, ends ) );
		}

		foreach( IPoint point in points ) {
			image[point.X, point.Y] = Color.LimeGreen;
		}

		image.SaveAsPng( Path.Combine( _folder, "hull.png" ) );
	}
}
