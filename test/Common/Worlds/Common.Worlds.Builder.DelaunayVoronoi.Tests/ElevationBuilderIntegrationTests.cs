using Common.Buffers;
using Kiyote.Geometry;
using Kiyote.Geometry.DelaunayVoronoi;
using Kiyote.Geometry.Rasterizers;
using Point = Kiyote.Geometry.Point;

namespace Common.Worlds.Builder.DelaunayVoronoi.Tests;

[TestFixture]
internal sealed class ElevationBuilderIntegrationTests {

	private ILandformBuilder _landformBuilder;
	private IBufferFactory _bufferFactory;
	private IRasterizer _rasterizer;
	private ISaltwaterBuilder _saltwaterBuilder;
	private IFreshwaterBuilder _freshwaterBuilder;
	private ILakeBuilder _lakeBuilder;
	private ElevationBuilder _builder;

	private IServiceProvider _provider;
	private IServiceScope _scope;
	private string _folder;

	[OneTimeSetUp]
	public void OneTimeSetUp() {
		string rootPath = Path.Combine( Path.GetTempPath(), "world" );
		_folder = Path.Combine( rootPath, nameof( ElevationBuilderIntegrationTests ) );
		Directory.CreateDirectory( _folder );
		var services = new ServiceCollection();
		services.AddDelaunayVoronoiWorldBuilder();
		services.AddRasterizer();
		services.AddCommonBuffers();

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
		_lakeBuilder = _scope.ServiceProvider.GetRequiredService<ILakeBuilder>();

		_builder = new ElevationBuilder();
	}

	[TearDown]
	public void TearDown() {
		_scope.Dispose();
	}

	[Test]
	[Ignore( "Used to visualize output for inspection." )]
	public async Task Visualize() {
		ISize size = new Point( 1000, 1000 );
		IReadOnlySet<Cell> landform = _landformBuilder.Create( size, out ISearchableVoronoi map );
		IReadOnlySet<Cell> saltwater = _saltwaterBuilder.Create( size, map, landform );
		IReadOnlySet<Cell> freshwater = _freshwaterBuilder.Create( size, map, landform, saltwater );
		IReadOnlyList<IReadOnlySet<Cell>> lakes = _lakeBuilder.Create( size, map, landform, saltwater, freshwater );
		IReadOnlyDictionary<Cell, float> elevation = ( _builder as IElevationBuilder ).Create( size, map, landform, saltwater, freshwater, lakes );

		float maximum = elevation.Max( kvp => kvp.Value );

		IBuffer<float> buffer = _bufferFactory.Create<float>( size );

		foreach( Cell cell in landform ) {
			if (!elevation.TryGetValue(cell, out float intensity)) {
				intensity = 0.0f;
			}
			_rasterizer.Rasterize( cell.Polygon.Points, ( int x, int y ) => {
				buffer[x, y] = ( intensity / maximum * 0.8f ) + 0.2f;
			} );
		}

		foreach (HashSet<Cell> lake in lakes) {
			foreach( Cell cell in lake ) {
				if( !elevation.TryGetValue( cell, out float intensity ) ) {
					intensity = 0.0f;
				}
				_rasterizer.Rasterize( cell.Polygon.Points, ( int x, int y ) => {
					buffer[x, y] = ( intensity / maximum * 0.8f ) + 0.2f;
				} );
			}
		}

		foreach( Edge edge in map.Edges ) {
			_rasterizer.Rasterize( edge.A, edge.B, ( int x, int y ) => {
				buffer[x, y] = 0.2f;
			} );
		}

		IBufferWriter<float> writer = new ImageBufferWriter( Path.Combine( _folder, "elevation.png" ) );
		await writer.WriteAsync( buffer );
	}
}
