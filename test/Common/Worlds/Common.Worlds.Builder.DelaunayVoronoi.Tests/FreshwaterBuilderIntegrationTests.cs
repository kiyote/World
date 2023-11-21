using Common.Buffers;
using Kiyote.Geometry;
using Kiyote.Geometry.DelaunayVoronoi;
using Kiyote.Geometry.Rasterizers;
using Point = Kiyote.Geometry.Point;

namespace Common.Worlds.Builder.DelaunayVoronoi.Tests;

[TestFixture]
internal sealed class FreshwaterBuilderIntegrationTests {

	private ILandformBuilder _landformBuilder;
	private IBufferFactory _bufferFactory;
	private IRasterizer _rasterizer;
	private ISaltwaterBuilder _saltwaterBuilder;
	private FreshwaterBuilder _builder;

	private IServiceProvider _provider;
	private IServiceScope _scope;
	private string _folder;

	[OneTimeSetUp]
	public void OneTimeSetUp() {
		string rootPath = Path.Combine( Path.GetTempPath(), "world" );
		_folder = Path.Combine( rootPath, nameof( FreshwaterBuilderIntegrationTests ) );
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

		_bufferFactory = _scope.ServiceProvider.GetRequiredService<IBufferFactory>();
		_rasterizer = _scope.ServiceProvider.GetRequiredService<IRasterizer>();
		_landformBuilder = _scope.ServiceProvider.GetRequiredService<ILandformBuilder>();
		_saltwaterBuilder = _scope.ServiceProvider.GetRequiredService<ISaltwaterBuilder>();

		_builder = new FreshwaterBuilder();
	}

	[TearDown]
	public void TearDown() {
		_scope.Dispose();
	}

	[Test]
	[Ignore( "Used to visualize output for inspection." )]
	public async Task Visualize() {
		ISize size = new Point( 1000, 1000 );
		HashSet<Cell> fineLandforms = _landformBuilder.Create( size, out ISearchableVoronoi voronoi );
		HashSet<Cell> oceans = _saltwaterBuilder.Create( size, voronoi, fineLandforms );
		HashSet<Cell> lakes = ( _builder as IFreshwaterBuilder ).Create( voronoi, fineLandforms, oceans );

		IBuffer<float> buffer = _bufferFactory.Create<float>( size );

		foreach( Cell cell in fineLandforms ) {
			_rasterizer.Rasterize( cell.Polygon.Points, ( int x, int y ) => {
				if( x >= 0 && x < size.Width && y >= 0 && y < size.Height ) {
					buffer[x, y] = 0.3f;
				}
			} );
		}

		foreach( Cell lake in lakes ) {
			_rasterizer.Rasterize( lake.Polygon.Points, ( int x, int y ) => {
				buffer[x, y] = 0.75f;
			} );
		}

		foreach( Edge edge in voronoi.Edges ) {
			_rasterizer.Rasterize( edge.A, edge.B, ( int x, int y ) => {
				buffer[x, y] = 0.2f;
			} );
		}

		IBufferWriter<float> writer = new ImageBufferWriter( Path.Combine( _folder, "freshwater.png" ) );
		await writer.WriteAsync( buffer );
	}
}
