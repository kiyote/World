using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Common.Worlds.Builder.Algorithms.DelaunayVoronoi;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;

namespace Common.Worlds.Builder.Generators.Tests;

[TestFixture]
public sealed class VoronoiHeightmapIntegrationTests {

	private string _folder;
	private IRandom _random;
	private IServiceScope _scope;
	private IServiceProvider _provider;
	private VoronoiHeightmap _heightmapper;

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

		_heightmapper = new VoronoiHeightmap(
			_random
		);
	}

	[TearDown]
	public void TearDown() {
		_scope.Dispose();
		_scope = null;
	}

	[Test]
	[Ignore("Used to generate visual output for inspection.")]
	public void Visualize() {

		IEnumerable<VoronoiRegion> voronoiRegions = _heightmapper.GenerateVoronoi( 1000, 1000 );
		Dictionary<VoronoiRegion, float> map = _heightmapper.Generate( 1000, 1000, voronoiRegions );

		Rgba32 ocean = new Rgba32( 0x00, 0x26, 0xFF );

		Rgba32[] colours = new Rgba32[] {
			new Rgba32( 0xFF, 0xB2, 0x7F ),
			new Rgba32( 0xDA, 0xFF, 0x7F ),
			new Rgba32( 0x52, 0x7F, 0x3F ),
			new Rgba32( 0x7F, 0x33, 0x00 ),
			new Rgba32( 0x7F, 0x00, 0x00 ),
			new Rgba32( 0xFF, 0xFF, 0xFF )
		};

		using Image<Rgba32> image = new Image<Rgba32>( 1000, 1000 );

		foreach( VoronoiRegion region in voronoiRegions) {
			if (!map.ContainsKey(region)) {
				DrawRegion( region, image, 500, 500, ocean );
			} else {
				float altitude = map[region];
				Rgba32 colour = colours[( Math.Clamp( (byte)(altitude * colours.Length) - 1, 0, colours.Length - 1 ) )];
				DrawRegion( region, image, 500, 500, colour );
			}
		}

		image.SaveAsPng( Path.Combine( _folder, "voronoi.png" ) );
	}

	private static void DrawRegion(
		VoronoiRegion region,
		Image<Rgba32> image,
		float xSize,
		float ySize,
		Rgba32 colour
	) {
		PointF[] points = new PointF[2];
		foreach( Edge edge in region.Edges ) {
			Vertex v0 = edge.From.CircumCenter;
			Vertex v1 = edge.To.CircumCenter;

			points[0].X = (float)v0.X + xSize;
			points[0].Y = (float)v0.Y + ySize;
			points[1].X = (float)v1.X + xSize;
			points[1].Y = (float)v1.Y + ySize;
			image.Mutate( i => i.DrawLines( colour, 1.0f, points ) );
		}
	}
}
