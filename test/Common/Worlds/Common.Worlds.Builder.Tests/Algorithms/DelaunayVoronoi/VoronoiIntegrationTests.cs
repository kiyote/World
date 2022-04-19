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
		int NumberOfVertices = 4000;
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

		foreach (Vertex v in vertices) {
			image[(int)( v.X + size ), (int)( v.Y + size )] = Color.LimeGreen;
		}

		// Filter out any regions where the cell's circumcenter isn't visible
		IEnumerable<VoronoiRegion> visibleRegions = voronoi.Regions.Where( r => !r.Cells.Where( c => !InBound( c.CircumCenter, size ) ).Any() );
		foreach (VoronoiRegion region in visibleRegions) {

			DrawRegion( region, image, size );

			/*
			List<Region> neighbours = new List<Region>();
			foreach( Edge edge in region.Edges) {
				neighbours.AddRange(
					visibleRegions
						.Where( vr => vr.Edges.Contains( edge ) )
						.Where( vr => vr != region )
						.Where( vr => !neighbours.Contains( vr ) )
						.Distinct()
				);
			}

			foreach (Region neighbour in neighbours) {
				DrawRegion( neighbour, image, size );
			}
			*/
		}

		image.Save( Path.Combine( _folder, "voronoi.png" ) );
	}

	private void DrawRegion(
		VoronoiRegion region,
		Image<Rgba32> image,
		float size
	) {
		Rgba32 colour = new Rgba32(
			(byte)( 128 + _random.NextInt( 128 ) ),
			(byte)( 128 + _random.NextInt( 128 ) ),
			(byte)( 128 + _random.NextInt( 128 ) )
		);
		PointF[] points = new PointF[2];
		foreach( Edge edge in region.Edges ) {
			Vertex v0 = edge.From.CircumCenter;
			Vertex v1 = edge.To.CircumCenter;

			points[0].X = (float)v0.X + size;
			points[0].Y = (float)v0.Y + size;
			points[1].X = (float)v1.X + size;
			points[1].Y = (float)v1.Y + size;
			image.Mutate( i => i.DrawLines( colour, 1.0f, points ) );
		}
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
