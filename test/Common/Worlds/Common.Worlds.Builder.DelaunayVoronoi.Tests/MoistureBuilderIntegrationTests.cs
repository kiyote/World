using Common.Buffers;
using Common.Geometry;
using Common.Geometry.DelaunayVoronoi;

namespace Common.Worlds.Builder.DelaunayVoronoi.Tests;

[TestFixture]
internal sealed class MoistureBuilderIntegrationTests {

	private ILandformBuilder _landformBuilder;
	private IBufferFactory _bufferFactory;
	private IGeometry _geometry;
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
		services.AddCommonCore();
		services.AddCommonBuffers();
		services.AddCommonGeometry();
		services.AddCommonWorlds();

		services.AddCommonWorldsBuilderDelaunayVoronoi();

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
		_geometry = _scope.ServiceProvider.GetRequiredService<IGeometry>();
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
		Size size = new Size( 1000, 1000 );
		HashSet<Cell> fineLandforms = _landformBuilder.Create( size, out ISearchableVoronoi voronoi );
		HashSet<Cell> oceans = _saltwaterBuilder.Create( size, voronoi, fineLandforms );
		HashSet<Cell> lakes = _freshwaterBuilder.Create( voronoi, fineLandforms, oceans );
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
			_geometry.RasterizePolygon( cell.Points, ( int x, int y ) => {
				if( x >= 0 && x < size.Columns && y >= 0 && y < size.Rows ) {
					buffer[x, y] = value;
				}
			} );
		}

		foreach( Edge edge in voronoi.Edges ) {
			_geometry.RasterizeLine( edge.A, edge.B, ( int x, int y ) => {
				if( x >= 0 && x < size.Columns && y >= 0 && y < size.Rows ) {
					buffer[x, y] = 0.2f;
				}
			} );
		}

		IBufferWriter<float> writer = new ImageBufferWriter( Path.Combine( _folder, "moisture.png" ) );
		await writer.WriteAsync( buffer );
	}
}
