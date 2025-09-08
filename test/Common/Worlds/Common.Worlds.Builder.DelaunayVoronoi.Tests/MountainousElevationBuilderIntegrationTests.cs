using System.Diagnostics;
using Kiyote.Buffers;
using Kiyote.Buffers.Float;
using Kiyote.Geometry;
using Kiyote.Geometry.DelaunayVoronoi;
using Kiyote.Geometry.Rasterizers;
using Point = Kiyote.Geometry.Point;

namespace Common.Worlds.Builder.DelaunayVoronoi.Tests;

[TestFixture]
internal sealed class MountainousElevationBuilderIntegrationTests : IBuilderMonitor {

	private ILandformBuilder _landformBuilder;
	private IBufferFactory _bufferFactory;
	private IRasterizer _rasterizer;
	private ISaltwaterFinder _saltwaterFinder;
	private IFreshwaterFinder _freshwaterBuilder;
	private ILakeFinder _lakeFinder;
	private IInlandDistanceBuilder _inlandDistanceBuilder;
	private ITectonicPlateBuilder _tectonicPlateBuilder;
	private ICoastFinder _coastFinder;
	private IFloatBufferOperators _operators;
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
		services.AddFloatBuffers();

		services.AddSingleton<IBuilderMonitor>( this );

		_provider = services.BuildServiceProvider();
		_operators = _provider.GetRequiredService<IFloatBufferOperators>();
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

		_bufferFactory = _scope.ServiceProvider.GetRequiredService<IBufferFactory>();
		_rasterizer = _scope.ServiceProvider.GetRequiredService<IRasterizer>();
		_landformBuilder = _scope.ServiceProvider.GetRequiredService<ILandformBuilder>();
		_saltwaterFinder = _scope.ServiceProvider.GetRequiredService<ISaltwaterFinder>();
		_freshwaterBuilder = _scope.ServiceProvider.GetRequiredService<IFreshwaterFinder>();
		_lakeFinder = _scope.ServiceProvider.GetRequiredService<ILakeFinder>();
		_tectonicPlateBuilder = _scope.ServiceProvider.GetRequiredService<ITectonicPlateBuilder>();
		_inlandDistanceBuilder = _scope.ServiceProvider.GetRequiredService<IInlandDistanceBuilder>();
		_coastFinder = _scope.ServiceProvider.GetRequiredService<ICoastFinder>();

		_builder = new MountainousElevationBuilder();
	}

	[TearDown]
	public void TearDown() {
		_scope.Dispose();
	}

	[Test]
	[Ignore( "Used to visualize output for inspection." )]
	public async Task Visualize() {
		ISize size = new Point( 1920, 1080 );
		TectonicPlates tectonicPlates = _tectonicPlateBuilder.Create( size );
		Landform landform = await _landformBuilder.CreateAsync( size, tectonicPlates, TestContext.CurrentContext.CancellationToken );
		IReadOnlySet<Cell> saltwater = _saltwaterFinder.Find( size, landform.Map, landform.Cells );
		IReadOnlySet<Cell> freshwater = _freshwaterBuilder.Create( size, landform.Map, landform.Cells, saltwater );
		IReadOnlyList<IReadOnlySet<Cell>> lakes = _lakeFinder.Finder( size, landform.Map, landform.Cells, saltwater, freshwater );
		IReadOnlySet<Cell> coast = _coastFinder.Find( size, landform.Map, landform.Cells, saltwater );
		IReadOnlyDictionary<Cell, float> inlandDistance = _inlandDistanceBuilder.Create( size, landform.Map, landform.Cells, coast );
		IReadOnlyDictionary<Cell, float> elevation = ( _builder as IElevationBuilder ).Create( size, tectonicPlates, landform.Map, landform.Cells, inlandDistance );

		float maximum = elevation.Max( kvp => kvp.Value );

		IBuffer<float> buffer = _bufferFactory.Create<float>( size );

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
		_operators.Normalize( buffer, 0.0f, 1.0f );

		foreach( Edge edge in landform.Map.Edges ) {
			_rasterizer.Rasterize( edge.A, edge.B, ( int x, int y ) => {
				buffer[x, y] = 0.2f;
			} );
		}

		IBufferWriter<float> writer = new ImageBufferWriter( Path.Combine( _folder, "elevation.png" ) );
		await writer.WriteAsync( buffer );
	}

	async Task IBuilderMonitor.LandformStageAsync(
		ISize size,
		int stage,
		IReadOnlySet<Cell> cells,
		CancellationToken cancellationToken
	) {
		IBuffer<float> buffer = _bufferFactory.Create<float>( size );
		foreach( Cell cell in cells ) {
			_rasterizer.Rasterize( cell.Polygon.Points, ( int x, int y ) => {
				buffer[x, y] = 0.5f;
			} );
		}

		foreach( Cell cell in cells ) {
			foreach (Edge edge in cell.Polygon.Edges) {
				_rasterizer.Rasterize( edge.A, edge.B, ( int x, int y ) => {
					buffer[x, y] = 0.25f;
				} );
			}
		}

		IBufferWriter<float> writer = new ImageBufferWriter( Path.Combine( _folder, $"elevation_stage{stage}.png" ) );
		await writer.WriteAsync( buffer );
	}

	async Task IBuilderMonitor.LandformStageMessageAsync( string message, CancellationToken cancellationToken ) {
		Debug.WriteLine( message );
		await TestContext.Progress.WriteLineAsync( message ).ConfigureAwait( false );
	}
}
