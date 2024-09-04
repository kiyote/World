using Common.Buffers;
using Kiyote.Geometry;
using Kiyote.Geometry.DelaunayVoronoi;
using Kiyote.Geometry.Rasterizers;
using Point = Kiyote.Geometry.Point;

namespace Common.Worlds.Builder.DelaunayVoronoi.Tests;

[TestFixture]
internal sealed class SaltwaterBuilderIntegrationTests {

	private ILandformBuilder _landformBuilder;
	private IBufferFactory _bufferFactory;
	private IRasterizer _rasterizer;
	private SaltwaterBuilder _builder;

	private IServiceProvider _provider;
	private IServiceScope _scope;
	private string _folder;

	[OneTimeSetUp]
	public void OneTimeSetUp() {
		string rootPath = Path.Combine( Path.GetTempPath(), "world" );
		_folder = Path.Combine( rootPath, nameof( SaltwaterBuilderIntegrationTests ) );
		Directory.CreateDirectory( _folder );
		var services = new ServiceCollection();
		services.AddCommonBuffers();
		services.AddCommonWorlds();
		services.AddDelaunayVoronoiWorldBuilder();
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

		_bufferFactory = _scope.ServiceProvider.GetRequiredService<IBufferFactory>();
		_rasterizer = _scope.ServiceProvider.GetRequiredService<IRasterizer>();
		_landformBuilder = _scope.ServiceProvider.GetRequiredService<ILandformBuilder>();

		_builder = new SaltwaterBuilder();
	}

	[TearDown]
	public void TearDown() {
		_scope.Dispose();
	}

	[Test]
	[Ignore( "Used to visualize output for inspection." )]
	public async Task Visualize() {
		ISize size = new Point( 1000, 1000 );
		HashSet<Cell> landform = _landformBuilder.Create( size, out ISearchableVoronoi map );
		HashSet<Cell> saltwater = ( _builder as ISaltwaterBuilder ).Create( size, map, landform );

		IBuffer<float> buffer = _bufferFactory.Create<float>( size );


		foreach( Cell cell in landform ) {
			_rasterizer.Rasterize( cell.Polygon.Points, ( int x, int y ) => {
				buffer[x, y] = 0.3f;
			} );
		}

		foreach( Cell cell in saltwater ) {
			_rasterizer.Rasterize( cell.Polygon.Points, ( int x, int y ) => {
				buffer[x, y] = 1.0f;
			} );
		}

		foreach( Edge edge in map.Edges ) {
			_rasterizer.Rasterize( edge.A, edge.B, ( int x, int y ) => {
				buffer[x, y] = 0.2f;
			} );
		}

		IBufferWriter<float> writer = new ImageBufferWriter( Path.Combine( _folder, "saltwater.png" ) );
		await writer.WriteAsync( buffer );
	}
}
