using System.Diagnostics;
using Kiyote.Buffers;
using Kiyote.Buffers.Numerics;
using Kiyote.Geometry;
using Kiyote.Geometry.DelaunayVoronoi;
using Kiyote.Geometry.Noises;
using Kiyote.Geometry.Rasterizers;
using Point = Kiyote.Geometry.Point;

namespace Common.Worlds.Builder.DelaunayVoronoi.Tests;

[TestFixture]
internal sealed class MountainousElevationBuilderIntegrationTests {

	private ILandformBuilder _landformBuilder;
	private INumericBufferFactory _bufferFactory;
	private IRasterizer _rasterizer;
	private ISaltwaterFinder _saltwaterFinder;
	private IFreshwaterFinder _freshwaterBuilder;
	private ILakeFinder _lakeFinder;
	private IInlandDistanceFinder _inlandDistanceBuilder;
	private ITectonicPlateBuilder _tectonicPlateBuilder;
	private ICoastFinder _coastFinder;
	private INumericBufferOperator _operators;
	private IElevationScaler _scaler;
	private IBufferOperator _ops;
	private MountainousElevationBuilder _builder;

	private IServiceProvider _provider;
	private IServiceScope _scope;
	private string _folder;

	[OneTimeSetUp]
	public void OneTimeSetUp() {
		string rootPath = Path.Combine( Path.GetTempPath(), "world" );
		_folder = Path.Combine( rootPath, nameof( MountainousElevationBuilderIntegrationTests ) );
		Directory.CreateDirectory( _folder );
		var services = new ServiceCollection();
		services.AddDelaunayVoronoiWorldBuilder();
		services.AddRasterizer();
		services.AddBuffers();
		services.AddNumericBuffers();
		services.AddNoise();

		_provider = services.BuildServiceProvider();
		_operators = _provider.GetRequiredService<INumericBufferOperator>();
		_ops = _provider.GetRequiredService<IBufferOperator>();
		_scaler = _provider.GetRequiredService<IElevationScaler>();
	}

	[OneTimeTearDown]
	public void OneTimeTearDown() {
		Directory.Delete( _folder, true );
	}

	[SetUp]
	public void SetUp() {
		_scope = _provider.CreateScope();

		_bufferFactory = _scope.ServiceProvider.GetRequiredService<INumericBufferFactory>();
		_rasterizer = _scope.ServiceProvider.GetRequiredService<IRasterizer>();
		_landformBuilder = _scope.ServiceProvider.GetRequiredService<ILandformBuilder>();
		_saltwaterFinder = _scope.ServiceProvider.GetRequiredService<ISaltwaterFinder>();
		_freshwaterBuilder = _scope.ServiceProvider.GetRequiredService<IFreshwaterFinder>();
		_lakeFinder = _scope.ServiceProvider.GetRequiredService<ILakeFinder>();
		_tectonicPlateBuilder = _scope.ServiceProvider.GetRequiredService<ITectonicPlateBuilder>();
		_inlandDistanceBuilder = _scope.ServiceProvider.GetRequiredService<IInlandDistanceFinder>();
		_coastFinder = _scope.ServiceProvider.GetRequiredService<ICoastFinder>();
		INoisyEdgeFactory noisyEdgeFactory = _scope.ServiceProvider.GetRequiredService<INoisyEdgeFactory>();

		_builder = new MountainousElevationBuilder( noisyEdgeFactory );
	}

	[TearDown]
	public void TearDown() {
		_scope.Dispose();
	}

	[Test]
	[Ignore( "Used to visualize output for inspection." )]
	public async Task Visualize() {
		ISize size = new Point( 1920, 1080 ); // 7680, 4320 );
		TectonicPlates tectonicPlates = _tectonicPlateBuilder.Create( size );
		Landform landform = await _landformBuilder.CreateAsync( size, tectonicPlates, TestContext.CurrentContext.CancellationToken );
		IReadOnlySet<Cell> saltwater = _saltwaterFinder.Find( size, landform.Map, landform.Cells );
		IReadOnlySet<Cell> freshwater = _freshwaterBuilder.Create( size, landform.Map, landform.Cells, saltwater );
		IReadOnlyList<IReadOnlySet<Cell>> lakes = _lakeFinder.Finder( size, landform.Map, landform.Cells, saltwater, freshwater );
		IReadOnlySet<Cell> coast = _coastFinder.Find( size, landform.Map, landform.Cells, saltwater );
		IReadOnlyDictionary<Cell, float> inlandDistance = await _inlandDistanceBuilder.CreateAsync( size, landform.Map, landform.Cells, coast, TestContext.CurrentContext.CancellationToken );
		IReadOnlyDictionary<Cell, float> elevation = await ( _builder as IElevationBuilder ).CreateAsync( size, tectonicPlates, landform.Map, landform.Cells, inlandDistance, TestContext.CurrentContext.CancellationToken );

		float maximum = elevation.Max( kvp => kvp.Value );

		INumericBuffer<float> buffer = _bufferFactory.Create<float>( size.Width, size.Height, 0.0f );

		foreach( Cell cell in landform.Cells ) {
			if( !elevation.TryGetValue( cell, out float intensity ) ) {
				intensity = 0.0f;
			}
			_rasterizer.Rasterize( cell.Polygon.Points, ( int x, int y ) => {
				buffer[x, y] = ( intensity / maximum * 0.8f ) + 0.2f;
			} );
		}

		/*
		foreach (IReadOnlySet<Cell> lake in lakes) {
			foreach( Cell cell in lake ) {
				if( !elevation.TryGetValue( cell, out float intensity ) ) {
					intensity = 0.0f;
				}
				_rasterizer.Rasterize( cell.Polygon.Points, ( int x, int y ) => {
					buffer[x, y] = ( intensity / maximum * 0.8f ) + 0.2f;
				} );
			}
		}
		*/

		_ops.Perform( buffer, _scaler.Scale, buffer );
		_operators.Normalize( buffer );

		foreach( Edge edge in landform.Map.Edges ) {
			_rasterizer.Rasterize( edge.A, edge.B, ( int x, int y ) => {
				buffer[x, y] = 0.2f;
			} );
		}

		IBufferWriter<float> writer = new ImageBufferWriter( Path.Combine( _folder, "elevation.png" ) );
		await writer.WriteAsync( buffer );
	}
}
