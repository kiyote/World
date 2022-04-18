using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using VoronoiLib;
using VoronoiLib.Structures;
using Path = System.IO.Path;

namespace Common.Worlds.Builder.Generators.Voronoi.Tests;

[TestFixture]
public class FortunesAlgorithmIntegrationTests {

	private IServiceScope _scope;
	private IServiceProvider _provider;
	private string _folder;

	private IRandom _random;

	[OneTimeSetUp]
	public void OneTimeSetUp() {
		string testRoot = Path.Combine( Path.GetTempPath(), "world" );
		Directory.CreateDirectory( testRoot );
		_folder = Path.Combine( testRoot, Guid.NewGuid().ToString( "N" ) );
		Directory.CreateDirectory( _folder );

		var services = new ServiceCollection();
		services.AddCore();
		services.AddWorldBuilder();

		_provider = services.BuildServiceProvider();
	}

	[OneTimeTearDown]
	public void OneTimeTearDown() {
		Directory.Delete( _folder, true );
	}

	[SetUp]
	public void SetUp() {
		_scope = _provider.CreateScope();

		_random = _provider.GetRequiredService<IRandom>();
	}

	[TearDown]
	public void TearDown() {
		_scope.Dispose();
		_scope = null;
	}

	[Test]
	[Ignore( "Used to generate visual output for inspection." )]
	public void Visualize() {
		List<FortuneSite> sites = new List<FortuneSite>();
		for( int i = 0; i < 1000; i++ ) {
			sites.Add( new FortuneSite( _random.NextInt( 1000 ), _random.NextInt( 1000 ) ) );
		}

		LinkedList<VEdge> edges = FortunesAlgorithm.Run(
			sites,
			0, 0, 1000, 1000 );

		using Image<Rgba32> image = new Image<Rgba32>( 1000, 1000 );

		PointF[] points = new PointF[2];
		foreach( VEdge edge in edges ) {
			points[0].X = (float)edge.Start.X;
			points[0].Y = (float)edge.Start.Y;
			points[1].X = (float)edge.End.X;
			points[1].Y = (float)edge.End.Y;
			image.Mutate( i => i.DrawLines( Color.Red, 1.0f, points ) );
		}

		image.Save( Path.Combine( _folder, "voronoi.png" ) );
	}
}
