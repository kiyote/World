using Common.Buffers;
using Kiyote.Geometry;
using Kiyote.Geometry.DelaunayVoronoi;
using Kiyote.Geometry.Randomization;
using Kiyote.Geometry.Rasterizers;
using Point = Kiyote.Geometry.Point;

namespace Common.Worlds.Builder.DelaunayVoronoi.Tests;

[TestFixture]
internal sealed class MountainsBuilderIntegrationTests {

	private ILandformBuilder _landformBuilder;
	private IRandom _random;
	private IBufferFactory _bufferFactory;
	private IRasterizer _rasterizer;
	private IVoronoiFactory _voronoiFactory;
	private MountainsBuilder _builder;
	private IServiceProvider _provider;
	private IServiceScope _scope;
	private string _folder;

	[OneTimeSetUp]
	public void OneTimeSetUp() {
		string rootPath = Path.Combine( Path.GetTempPath(), "world" );
		_folder = Path.Combine( rootPath, nameof( MountainsBuilderIntegrationTests ) );
		Directory.CreateDirectory( _folder );
		var services = new ServiceCollection();
		services.AddCommonBuffers();
		services.AddCommonWorlds();
		services.AddCommonWorldsBuilderDelaunayVoronoi();
		services.AddRandomization();
		services.AddDelaunayVoronoi();
		services.AddRasterizer();

		_provider = services.BuildServiceProvider();

	}

	[OneTimeTearDown]
	public void OneTimeTearDown() {
		Directory.Delete( _folder, true );
	}

	[SetUp]
	public void SetUp() {
		_scope = _provider.CreateScope();

		_landformBuilder = _scope.ServiceProvider.GetRequiredService<ILandformBuilder>();
		_bufferFactory = _scope.ServiceProvider.GetRequiredService<IBufferFactory>();
		_random = _scope.ServiceProvider.GetRequiredService<IRandom>();
		_rasterizer = _scope.ServiceProvider.GetRequiredService<IRasterizer>();
		_voronoiFactory = _scope.ServiceProvider.GetRequiredService<IVoronoiFactory>();
		_builder = new MountainsBuilder(
			_scope.ServiceProvider.GetRequiredService<IRandom>(),
			_scope.ServiceProvider.GetRequiredService<IRasterizer>()
		);
	}

	[TearDown]
	public void TearDown() {
		_scope.Dispose();
	}

	[Test]
	[Ignore("Used to visualize output for inspection.")]
	public async Task Visualize() {
		ISize size = new Point( 1000, 1000 );
		HashSet<Cell> fineLandforms = _landformBuilder.Create( size, out ISearchableVoronoi voronoi );
		List<Cell> mountains = [];
		do {
			List<Edge> lines = _builder.GetMountainLines( size, size.Width / 100 );
			mountains.AddRange( _builder.BuildRanges( voronoi, fineLandforms, lines ) );
		} while( mountains.Count < ( size.Height / 10 ) );
		mountains = mountains.Distinct().ToList();

		IBuffer<float> buffer = _bufferFactory.Create<float>( size );


		foreach( Cell cell in fineLandforms ) {
			_rasterizer.Rasterize( cell.Polygon.Points, ( int x, int y ) => {
				if( x >= 0 && x < size.Width && y >= 0 && y < size.Height ) {
					buffer[x, y] = 0.3f;
				}
			} );
		}

		foreach( Cell mountain in mountains ) {
			_rasterizer.Rasterize( mountain.Polygon.Points, ( int x, int y ) => {
				buffer[x, y] = 0.6f;
			} );
		}

		foreach( Edge edge in voronoi.Edges ) {
			_rasterizer.Rasterize( edge.A, edge.B, ( int x, int y ) => {
				if( x >= 0 && x < size.Width && y >= 0 && y < size.Height ) {
					buffer[x, y] = 0.2f;
				}
			} );
		}

		IBufferWriter<float> writer = new ImageBufferWriter( Path.Combine( _folder, "mountains.png" ) );
		await writer.WriteAsync( buffer );
	}

	[Test]
	[Ignore( "Used to visualize output for inspection." )]
	public async Task Visualize_GetMountainLines() {
		ISize size = new Point( 1000, 1000 );
		List<Edge> lines = _builder.GetMountainLines( size, 10 );

		IBuffer<float> buffer = _bufferFactory.Create<float>( size );
		foreach( Edge edge in lines ) {
			_rasterizer.Rasterize( edge.A, edge.B, ( int x, int y ) => {
				buffer[x, y] = 1.0f;
			} );
		}

		IBufferWriter<float> writer = new ImageBufferWriter( Path.Combine( _folder, "lines.png" ) );
		await writer.WriteAsync( buffer );
	}
}
