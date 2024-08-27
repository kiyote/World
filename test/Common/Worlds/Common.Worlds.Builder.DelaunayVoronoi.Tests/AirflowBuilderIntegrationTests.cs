using Common.Buffers;
using Kiyote.Geometry;
using Kiyote.Geometry.DelaunayVoronoi;
using Kiyote.Geometry.Rasterizers;
using Point = Kiyote.Geometry.Point;

namespace Common.Worlds.Builder.DelaunayVoronoi.Tests;

[TestFixture]
internal sealed class AirflowBuilderIntegrationTests {

	private IRasterizer _rasterizer;
	private ILandformBuilder _landformBuilder;
	private IBufferFactory _bufferFactory;
	private ISaltwaterBuilder _saltwaterBuilder;
	private IFreshwaterBuilder _freshwaterBuilder;
	private IMountainsBuilder _mountainsBuilder;
	private IHillsBuilder _hillsBuilder;
	private AirflowBuilder _builder;

	private IServiceProvider _provider;
	private IServiceScope _scope;
	private string _folder;

	[OneTimeSetUp]
	public void OneTimeSetUp() {
		string rootPath = Path.Combine( Path.GetTempPath(), "world" );
		_folder = Path.Combine( rootPath, nameof( AirflowBuilderIntegrationTests ) );
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

		_rasterizer = _scope.ServiceProvider.GetRequiredService<IRasterizer>();
		_bufferFactory = _scope.ServiceProvider.GetRequiredService<IBufferFactory>();
		_landformBuilder = _scope.ServiceProvider.GetRequiredService<ILandformBuilder>();
		_mountainsBuilder = _scope.ServiceProvider.GetRequiredService<IMountainsBuilder>();
		_hillsBuilder = _scope.ServiceProvider.GetRequiredService<IHillsBuilder>();
		_saltwaterBuilder = _scope.ServiceProvider.GetRequiredService<ISaltwaterBuilder>();
		_freshwaterBuilder = _scope.ServiceProvider.GetRequiredService<IFreshwaterBuilder>();

		_builder = new AirflowBuilder();
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
		HashSet<Cell> hills = _hillsBuilder.Create( voronoi, fineLandforms, mountains );
		Dictionary<Cell, float> airflows = ( _builder as IAirflowBuilder ).Create(
			size,
			voronoi,
			fineLandforms,
			mountains,
			hills
		);

		IBuffer<float> buffer = _bufferFactory.Create<float>( size );

		foreach( Cell cell in airflows.Keys ) {
			float flow = airflows[cell];
			_rasterizer.Rasterize( cell.Polygon.Points, ( int x, int y ) => {
				if( x >= 0 && x < size.Width && y >= 0 && y < size.Height ) {
					buffer[x, y] = flow;
				}
			} );
		}

		foreach( Edge edge in voronoi.Edges ) {
			_rasterizer.Rasterize( edge.A, edge.B, ( int x, int y ) => {
				if( x >= 0 && x < size.Width && y >= 0 && y < size.Height ) {
					buffer[x, y] = 0.2f;
				}
			} );
		}

		IBufferWriter<float> writer = new ImageBufferWriter( Path.Combine( _folder, "airflow.png" ) );
		await writer.WriteAsync( buffer );
	}
}
