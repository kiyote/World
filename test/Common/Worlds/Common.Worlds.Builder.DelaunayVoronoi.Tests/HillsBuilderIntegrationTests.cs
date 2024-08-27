using Common.Buffers;
using Kiyote.Geometry;
using Kiyote.Geometry.DelaunayVoronoi;
using Kiyote.Geometry.Rasterizers;
using Point = Kiyote.Geometry.Point;

namespace Common.Worlds.Builder.DelaunayVoronoi.Tests;

[TestFixture]
internal sealed class HillsBuilderIntegrationTests {

	private ILandformBuilder _landformBuilder;
	private IBufferFactory _bufferFactory;
	private IRasterizer _rasterizer;
	private HillsBuilder _builder;
	private IMountainsBuilder _mountainsBuilder;
	private IServiceProvider _provider;
	private IServiceScope _scope;
	private string _folder;

	[OneTimeSetUp]
	public void OneTimeSetUp() {
		string rootPath = Path.Combine( Path.GetTempPath(), "world" );
		_folder = Path.Combine( rootPath, nameof( HillsBuilderIntegrationTests ) );
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
		_mountainsBuilder = _scope.ServiceProvider.GetRequiredService<IMountainsBuilder>();

		_builder = new HillsBuilder();
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
		HashSet<Cell> mountains = _mountainsBuilder.Create( size, voronoi, fineLandforms );
		HashSet<Cell> hills =  (_builder as IHillsBuilder).Create( voronoi, fineLandforms, mountains );

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
				buffer[x, y] = 0.75f;
			} );
		}

		foreach( Cell hill in hills ) {
			_rasterizer.Rasterize( hill.Polygon.Points, ( int x, int y ) => {
				buffer[x, y] = 1.0f;
			} );
		}

		foreach( Edge edge in voronoi.Edges ) {
			_rasterizer.Rasterize( edge.A, edge.B, ( int x, int y ) => {
				if( x >= 0 && x < size.Width && y >= 0 && y < size.Height ) {
					buffer[x, y] = 0.2f;
				}
			} );
		}

		IBufferWriter<float> writer = new ImageBufferWriter( Path.Combine( _folder, "hills.png" ) );
		await writer.WriteAsync( buffer );
	}
}
