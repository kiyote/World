using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Path = System.IO.Path;

namespace Common.Worlds.Builder.Algorithms.DelaunayVoronoi.Tests;

[TestFixture]
public sealed class VoronoiIntegrationTests {

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
		List<Vertex> vertices = new List<Vertex>();
		int NumberOfVertices = 1000;
		float size = 500;
		for( int i = 0; i < NumberOfVertices; i++ ) {
			float x = _random.NextFloat( -size, size );
			float y = _random.NextFloat( -size, size );

			vertices.Add( new Vertex( x, y ) );
		}
		IDelaunayFactory delaunayFactory = new DelaunayFactory();
		Delaunay delaunay = delaunayFactory.Create( vertices );
		IVoronoiFactory voronoiFactory = new VoronoiFactory();
		Voronoi voronoi = voronoiFactory.Create( delaunay );

		using Image<Rgba32> image = new Image<Rgba32>( (int)(size * 2.0f), (int)(size * 2.0f) );

		PointF[] points = new PointF[2];
		foreach (Vertex v in vertices) {
			image[(int)( v.X + size ), (int)( v.Y + size )] = Color.LimeGreen;
		}

		// Filter out any regions where the cell's circumcenter isn't visible
		IEnumerable<Region> visibleRegions = voronoi.Regions.Where( r => !r.Cells.Where( c => !InBound( c.CircumCenter, size ) ).Any() );
		foreach (Region region in visibleRegions) {

			foreach( Edge edge in region.Edges ) {
				Vertex v0 = edge.From.CircumCenter;
				Vertex v1 = edge.To.CircumCenter;

				points[0].X = (float)v0.X + size;
				points[0].Y = (float)v0.Y + size;
				points[1].X = (float)v1.X + size;
				points[1].Y = (float)v1.Y + size;
				image.Mutate( i => i.DrawLines( Color.Red, 1.0f, points ) );
			}
		}

		image.Save( Path.Combine( _folder, "voronoi.png" ) );
	}

	private static bool InBound( Vertex v, float size ) {
		if( v.X < -size || v.X > size ) {
			return false;
		}
		if( v.Y < -size || v.Y > size ) {
			return false;
		}

		return true;
	}
}
