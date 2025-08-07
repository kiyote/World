using Kiyote.Buffers;
using Kiyote.Buffers.Float;
using Kiyote.Geometry;
using Kiyote.Geometry.DelaunayVoronoi;
using Kiyote.Geometry.Rasterizers;

namespace Common.Worlds.Builder.DelaunayVoronoi.Tests;

internal sealed class LinearInlandDistanceBuilderIntegrationTests {

	private ILandformBuilder _landformBuilder;
	private IBufferFactory _bufferFactory;
	private IRasterizer _rasterizer;
	private ISaltwaterBuilder _saltwaterBuilder;
	private ICoastBuilder _coastBuilder;
	private IFloatBufferOperators _bufferOperator;
	private ITectonicPlateBuilder _tectonicPlateBuilder;
	private LinearInlandDistanceBuilder _builder;

	private IServiceProvider _provider;
	private IServiceScope _scope;
	private string _folder;

	[OneTimeSetUp]
	public void OneTimeSetUp() {
		string rootPath = Path.Combine( Path.GetTempPath(), "world" );
		_folder = Path.Combine( rootPath, nameof( LinearInlandDistanceBuilderIntegrationTests ) );
		Directory.CreateDirectory( _folder );
		var services = new ServiceCollection();
		services.AddDelaunayVoronoiWorldBuilder();
		services.AddRasterizer();
		services.AddFloatBuffers();

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
		_bufferOperator = _scope.ServiceProvider.GetService<IFloatBufferOperators>();
		_rasterizer = _scope.ServiceProvider.GetRequiredService<IRasterizer>();
		_landformBuilder = _scope.ServiceProvider.GetRequiredService<ILandformBuilder>();
		_saltwaterBuilder = _scope.ServiceProvider.GetRequiredService<ISaltwaterBuilder>();
		_coastBuilder = _scope.ServiceProvider.GetRequiredService<ICoastBuilder>();
		_tectonicPlateBuilder = _scope.ServiceProvider.GetRequiredService<ITectonicPlateBuilder>();

		_builder = new LinearInlandDistanceBuilder();
	}

	[TearDown]
	public void TearDown() {
		_scope.Dispose();
	}

	[Test]
	[Ignore( "Used to visualize output for inspection." )]
	public async Task Visualize() {
		ISize size = new Point( 1600, 900 );
		TectonicPlates tectonicPlates = _tectonicPlateBuilder.Create( size );
		IReadOnlySet<Cell> landform = _landformBuilder.Create( size, tectonicPlates, out ISearchableVoronoi map );
		IReadOnlySet<Cell> saltwater = _saltwaterBuilder.Create( size, map, landform );
		IReadOnlySet<Cell> coast = _coastBuilder.Create( size, map, landform, saltwater );
		IReadOnlyDictionary<Cell, float> elevation = ( _builder as IInlandDistanceBuilder ).Create( size, map, landform, coast );

		float maximum = elevation.Max( kvp => kvp.Value );

		IBuffer<float> buffer = _bufferFactory.Create<float>( size, 0.0f );

		foreach( Cell cell in landform ) {
			if( !elevation.TryGetValue( cell, out float intensity ) ) {
				intensity = 0.0f;
			}
			_rasterizer.Rasterize( cell.Polygon.Points, ( int x, int y ) => {
				buffer[x, y] = intensity;
			} );
		}

		_bufferOperator.Normalize( buffer, 0.0f, 1.0f );

		foreach( Edge edge in map.Edges ) {
			_rasterizer.Rasterize( edge.A, edge.B, ( int x, int y ) => {
				buffer[x, y] = 0.0f;
			} );
		}

		IBufferWriter<float> writer = new ImageBufferWriter( Path.Combine( _folder, "inland_distance.png" ) );
		await writer.WriteAsync( buffer );
	}

}
