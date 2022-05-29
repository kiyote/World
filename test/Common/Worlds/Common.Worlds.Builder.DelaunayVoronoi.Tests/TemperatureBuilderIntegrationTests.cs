using Common.Buffers;
using Common.Geometry;
using Common.Geometry.DelaunayVoronoi;

namespace Common.Worlds.Builder.DelaunayVoronoi.Tests;

[TestFixture]
internal sealed class TemperatureBuilderIntegrationTests {

	private ILandformBuilder _landformBuilder;
	private IBufferFactory _bufferFactory;
	private IGeometry _geometry;
	private ISaltwaterBuilder _saltwaterBuilder;
	private IFreshwaterBuilder _freshwaterBuilder;
	private IMountainsBuilder _mountainsBuilder;
	private IHillsBuilder _hillsBuilder;
	private TemperatureBuilder _builder;

	private IServiceProvider _provider;
	private IServiceScope _scope;
	private string _folder;

	[OneTimeSetUp]
	public void OneTimeSetUp() {
		string rootPath = Path.Combine( Path.GetTempPath(), "world" );
		_folder = Path.Combine( rootPath, nameof( TemperatureBuilderIntegrationTests ) );
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
		_mountainsBuilder = _scope.ServiceProvider.GetRequiredService<IMountainsBuilder>();
		_hillsBuilder = _scope.ServiceProvider.GetRequiredService<IHillsBuilder>();
		_saltwaterBuilder = _scope.ServiceProvider.GetRequiredService<ISaltwaterBuilder>();
		_freshwaterBuilder = _scope.ServiceProvider.GetRequiredService<IFreshwaterBuilder>();

		_builder = new TemperatureBuilder(
			_geometry
		);
	}

	[TearDown]
	public void TearDown() {
		_scope.Dispose();
	}

	[Test]
	[Ignore( "Used to visualize output for inspection." )]
	public async Task Visualize() {
		Size size = new Size( 1000, 1000 );
		HashSet<Cell> fineLandforms = _landformBuilder.Create( size, out Voronoi fineVoronoi );
		HashSet<Cell> mountains = _mountainsBuilder.Create( size, fineVoronoi, fineLandforms );
		HashSet<Cell> hills = _hillsBuilder.Create( fineVoronoi, fineLandforms, mountains );
		HashSet<Cell> oceans = _saltwaterBuilder.Create( size, fineVoronoi, fineLandforms );
		HashSet<Cell> lakes = _freshwaterBuilder.Create( fineVoronoi, fineLandforms, oceans );
		Dictionary<Cell, int> temperatures = ( _builder as ITemperatureBuilder ).Create(
			size,
			fineVoronoi,
			fineLandforms,
			mountains,
			hills,
			oceans,
			lakes
		);

		IBuffer<float> buffer = _bufferFactory.Create<float>( size );

		int maxValue = temperatures.Values.Max();
		foreach( Cell cell in temperatures.Keys ) {
			float temp = (float)temperatures[cell] / (float)maxValue;
			_geometry.RasterizePolygon( cell.Points, ( int x, int y ) => {
				if( x >= 0 && x < size.Columns && y >= 0 && y < size.Rows ) {
					buffer[x, y] = temp;
				}
			} );
		}

		foreach( Edge edge in fineVoronoi.Edges ) {
			_geometry.RasterizeLine( edge.A, edge.B, ( int x, int y ) => {
				if( x >= 0 && x < size.Columns && y >= 0 && y < size.Rows ) {
					buffer[x, y] = 0.2f;
				}
			} );
		}

		IBufferWriter<float> writer = new ImageBufferWriter( Path.Combine( _folder, "temperature.png" ) );
		await writer.WriteAsync( buffer );
	}
}
