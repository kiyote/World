using Kiyote.Buffers;
using Kiyote.Buffers.Numerics;
using Kiyote.Geometry;
using Kiyote.Geometry.DelaunayVoronoi;
using Kiyote.Geometry.Rasterizers;

namespace Common.Worlds.Builder.DelaunayVoronoi.Tests;

internal sealed class LinearInlandDistanceFinderIntegrationTests {

	private ILandformBuilder _landformBuilder;
	private INumericBufferFactory _bufferFactory;
	private IRasterizer _rasterizer;
	private ISaltwaterFinder _saltwaterBuilder;
	private ICoastFinder _coastBuilder;
	private INumericBufferOperator _bufferOperator;
	private ITectonicPlateBuilder _tectonicPlateBuilder;
	private LinearInlandDistanceBuilder _builder;

	private IServiceProvider _provider;
	private IServiceScope _scope;
	private string _folder;

	[OneTimeSetUp]
	public void OneTimeSetUp() {
		string rootPath = Path.Combine( Path.GetTempPath(), "world" );
		_folder = Path.Combine( rootPath, nameof( LinearInlandDistanceFinderIntegrationTests ) );
		Directory.CreateDirectory( _folder );
		var services = new ServiceCollection();
		services.AddDelaunayVoronoiWorldBuilder();
		services.AddRasterizer();
		services.AddNumericBuffers();

		_provider = services.BuildServiceProvider();

	}

	[OneTimeTearDown]
	public void OneTimeTearDown() {
		Directory.Delete( _folder, true );
	}

	[SetUp]
	public void SetUp() {
		_scope = _provider.CreateScope();

		_bufferFactory = _scope.ServiceProvider.GetRequiredService<INumericBufferFactory>();
		_bufferOperator = _scope.ServiceProvider.GetService<INumericBufferOperator>();
		_rasterizer = _scope.ServiceProvider.GetRequiredService<IRasterizer>();
		_landformBuilder = _scope.ServiceProvider.GetRequiredService<ILandformBuilder>();
		_saltwaterBuilder = _scope.ServiceProvider.GetRequiredService<ISaltwaterFinder>();
		_coastBuilder = _scope.ServiceProvider.GetRequiredService<ICoastFinder>();
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
		Landform landform = await _landformBuilder.CreateAsync( size, tectonicPlates, TestContext.CurrentContext.CancellationToken );
		IReadOnlySet<Cell> saltwater = _saltwaterBuilder.Find( size, landform.Map, landform.Cells );
		IReadOnlySet<Cell> coast = _coastBuilder.Find( size, landform.Map, landform.Cells, saltwater );
		IReadOnlyDictionary<Cell, float> elevation = ( _builder as IInlandDistanceBuilder ).Create( size, landform.Map, landform.Cells, coast );

		float maximum = elevation.Max( kvp => kvp.Value );

		INumericBuffer<float> buffer = _bufferFactory.Create<float>( size.Width, size.Height, 0.0f );

		foreach( Cell cell in landform.Cells ) {
			if( !elevation.TryGetValue( cell, out float intensity ) ) {
				intensity = 0.0f;
			}
			_rasterizer.Rasterize( cell.Polygon.Points, ( int x, int y ) => {
				buffer[x, y] = intensity;
			} );
		}

		_bufferOperator.Normalize( buffer );

		foreach( Edge edge in landform.Map.Edges ) {
			_rasterizer.Rasterize( edge.A, edge.B, ( int x, int y ) => {
				buffer[x, y] = 0.0f;
			} );
		}

		IBufferWriter<float> writer = new ImageBufferWriter( Path.Combine( _folder, "inland_distance.png" ) );
		await writer.WriteAsync( buffer );
	}

}
