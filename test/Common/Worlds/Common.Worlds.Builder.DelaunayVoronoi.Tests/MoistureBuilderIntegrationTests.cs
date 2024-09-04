using Common.Buffers;
using Kiyote.Geometry;
using Kiyote.Geometry.DelaunayVoronoi;
using Kiyote.Geometry.Rasterizers;
using Point = Kiyote.Geometry.Point;

namespace Common.Worlds.Builder.DelaunayVoronoi.Tests;

[TestFixture]
internal sealed class MoistureBuilderIntegrationTests {

	private ILandformBuilder _landformBuilder;
	private IBufferFactory _bufferFactory;
	private IRasterizer _rasterizer;
	private ISaltwaterBuilder _saltwaterBuilder;
	private IFreshwaterBuilder _freshwaterBuilder;
	private IAirflowBuilder _airflowBuilder;
	private IMountainsBuilder _mountainsBuilder;
	private IHillsBuilder _hillsBuilder;
	private ITemperatureBuilder _temperatureBuilder;
	private MoistureBuilder _builder;

	private IServiceProvider _provider;
	private IServiceScope _scope;
	private string _folder;

	[OneTimeSetUp]
	public void OneTimeSetUp() {
		string rootPath = Path.Combine( Path.GetTempPath(), "world" );
		_folder = Path.Combine( rootPath, nameof( MoistureBuilderIntegrationTests ) );
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
		_saltwaterBuilder = _scope.ServiceProvider.GetRequiredService<ISaltwaterBuilder>();
		_freshwaterBuilder = _scope.ServiceProvider.GetRequiredService<IFreshwaterBuilder>();
		_airflowBuilder = _scope.ServiceProvider.GetRequiredService<IAirflowBuilder>();
		_mountainsBuilder = _scope.ServiceProvider.GetRequiredService<IMountainsBuilder>();
		_hillsBuilder = _scope.ServiceProvider.GetRequiredService<IHillsBuilder>();
		_temperatureBuilder = _scope.ServiceProvider.GetRequiredService<ITemperatureBuilder>();

		_builder = new MoistureBuilder();
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
		HashSet<Cell> lakes = _freshwaterBuilder.Create( size, voronoi, fineLandforms, oceans );
		HashSet<Cell> mountains = _mountainsBuilder.Create( size, voronoi, fineLandforms );
		HashSet<Cell> hills = _hillsBuilder.Create( voronoi, fineLandforms, mountains );
		Dictionary<Cell, float> temperatures = _temperatureBuilder.Create( size, voronoi, fineLandforms, mountains, hills, oceans, lakes );
		Dictionary<Cell, float> winds = _airflowBuilder.Create( size, voronoi, fineLandforms, mountains, hills );
		Dictionary<Cell, float> rains = ( _builder as IMoistureBuilder ).Create(
			size,
			voronoi,
			fineLandforms,
			oceans,
			lakes,
			temperatures,
			winds
		);

		IBuffer<float> buffer = _bufferFactory.Create<float>( size );

		foreach( Cell cell in rains.Keys ) {
			float value = rains[cell];
			_rasterizer.Rasterize( cell.Polygon.Points, ( int x, int y ) => {
				if( x >= 0 && x < size.Width && y >= 0 && y < size.Height ) {
					buffer[x, y] = value;
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

		IBufferWriter<float> writer = new ImageBufferWriter( Path.Combine( _folder, "moisture.png" ) );
		await writer.WriteAsync( buffer );
	}
}
