using Kiyote.Buffers;
using Kiyote.Geometry;
using Kiyote.Geometry.DelaunayVoronoi;
using Kiyote.Geometry.Rasterizers;

namespace Common.Worlds.Builder.DelaunayVoronoi.Tests;

[TestFixture]
internal sealed class LakeFinderIntegrationTests {

	private ILandformBuilder _landformBuilder;
	private ISaltwaterFinder _saltwaterBuilder;
	private IFreshwaterFinder _freshwaterBuilder;
	private ITectonicPlateBuilder _tectonicPlateBuilder;
	private ILakeFinder _builder;
	private IBufferFactory _bufferFactory;
	private IRasterizer _rasterizer;

	private IServiceProvider _provider;
	private IServiceScope _scope;
	private string _folder;

	[OneTimeSetUp]
	public void OneTimeSetUp() {
		_folder = Path.Combine( Path.GetTempPath(), "world", nameof( LakeFinderIntegrationTests ) );
		Directory.CreateDirectory( _folder );
		var services = new ServiceCollection();
		services.AddRasterizer();
		services.AddBuffers();
		services.AddDelaunayVoronoiWorldBuilder();

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
		_tectonicPlateBuilder = _scope.ServiceProvider.GetRequiredService<ITectonicPlateBuilder>();

		_builder = new LandlockedLakeFinder(
		);
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
		IReadOnlyList<IReadOnlySet<Cell>> lakes = _builder.Finder( size, map, landform, saltwater, freshwater );

		IBuffer<float> buffer = _bufferFactory.Create<float>( size );

		foreach( Cell cell in landform ) {
			_rasterizer.Rasterize( cell.Polygon.Points, ( int x, int y ) => {
				buffer[x, y] = 0.3f;
			} );
		}

		foreach( Cell cell in freshwater ) {
			_rasterizer.Rasterize( cell.Polygon.Points, ( int x, int y ) => {
				buffer[x, y] = 0.5f;
			} );
		}

		foreach( IReadOnlySet<Cell> lake in lakes) {
			foreach( Cell cell in lake ) {
				_rasterizer.Rasterize( cell.Polygon.Points, ( int x, int y ) => {
					buffer[x, y] = 1.0f;
				} );
			}
		}

		foreach( Edge edge in map.Edges ) {
			_rasterizer.Rasterize( edge.A, edge.B, ( int x, int y ) => {
				buffer[x, y] = 0.2f;
			} );
		}

		IBufferWriter<float> writer = new ImageBufferWriter( Path.Combine( _folder, "lakes.png" ) );
		await writer.WriteAsync( buffer );
	}
}
