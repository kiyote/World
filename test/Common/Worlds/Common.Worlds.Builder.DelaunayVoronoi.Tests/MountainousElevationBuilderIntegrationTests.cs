using Kiyote.Buffers;
using Kiyote.Geometry;
using Kiyote.Geometry.DelaunayVoronoi;
using Kiyote.Geometry.Rasterizers;
using Point = Kiyote.Geometry.Point;

namespace Common.Worlds.Builder.DelaunayVoronoi.Tests;

[TestFixture]
internal sealed class MountainousElevationBuilderIntegrationTests {

	private ILandformBuilder _landformBuilder;
	private IBufferFactory _bufferFactory;
	private IRasterizer _rasterizer;
	private ISaltwaterFinder _saltwaterBuilder;
	private IFreshwaterFinder _freshwaterBuilder;
	private ILakeFinder _lakeBuilder;
	private IInlandDistanceBuilder _inlandDistanceBuilder;
	private ITectonicPlateBuilder _tectonicPlateBuilder;
	private ICoastFinder _coastBuilder;
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
		_saltwaterBuilder = _scope.ServiceProvider.GetRequiredService<ISaltwaterFinder>();
		_freshwaterBuilder = _scope.ServiceProvider.GetRequiredService<IFreshwaterFinder>();
		_lakeBuilder = _scope.ServiceProvider.GetRequiredService<ILakeFinder>();
		_tectonicPlateBuilder = _scope.ServiceProvider.GetRequiredService<ITectonicPlateBuilder>();
		_inlandDistanceBuilder = _scope.ServiceProvider.GetRequiredService<IInlandDistanceBuilder>();
		_coastBuilder = _scope.ServiceProvider.GetRequiredService<ICoastFinder>();

		_builder = new MountainousElevationBuilder();
	}

	[TearDown]
	public void TearDown() {
		_scope.Dispose();
	}

	[Test]
	[Ignore( "Used to visualize output for inspection." )]
	public async Task Visualize() {
		ISize size = new Point( 1000, 1000 );
		TectonicPlates tectonicPlates = _tectonicPlateBuilder.Create( size );
		IReadOnlySet<Cell> landform = _landformBuilder.Create( size, tectonicPlates, out ISearchableVoronoi map );
		IReadOnlySet<Cell> saltwater = _saltwaterBuilder.Find( size, map, landform );
		IReadOnlySet<Cell> freshwater = _freshwaterBuilder.Create( size, map, landform, saltwater );
		IReadOnlyList<IReadOnlySet<Cell>> lakes = _lakeBuilder.Finder( size, map, landform, saltwater, freshwater );
		IReadOnlySet<Cell> coast = _coastBuilder.Find( size, map, landform, saltwater );
		IReadOnlyDictionary<Cell, float> inlandDistance = _inlandDistanceBuilder.Create( size, map, landform, coast );
		IReadOnlyDictionary<Cell, float> elevation = ( _builder as IElevationBuilder ).Create( size, tectonicPlates, map, landform, inlandDistance );

		float maximum = elevation.Max( kvp => kvp.Value );

		IBuffer<float> buffer = _bufferFactory.Create<float>( size );

		foreach( Cell cell in landform ) {
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

		foreach( Edge edge in map.Edges ) {
			_rasterizer.Rasterize( edge.A, edge.B, ( int x, int y ) => {
				buffer[x, y] = 0.2f;
			} );
		}

		IBufferWriter<float> writer = new ImageBufferWriter( Path.Combine( _folder, "elevation.png" ) );
		await writer.WriteAsync( buffer );
	}
}
