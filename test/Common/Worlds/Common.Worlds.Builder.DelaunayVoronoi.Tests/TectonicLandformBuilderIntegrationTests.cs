using Kiyote.Buffers;
using Kiyote.Geometry;
using Kiyote.Geometry.DelaunayVoronoi;
using Kiyote.Geometry.Rasterizers;
using Point = Kiyote.Geometry.Point;

namespace Common.Worlds.Builder.DelaunayVoronoi.Tests;

[TestFixture]
public sealed class TectonicLandformBuilderIntegrationTests {

	private IRasterizer _rasterizer;
	private ILandformBuilder _landformBuilder;
	private IBufferFactory _bufferFactory;

	private IServiceProvider _provider;
	private IServiceScope _scope;
	private string _folder;

	[OneTimeSetUp]
	public void OneTimeSetUp() {
		string rootPath = Path.Combine( Path.GetTempPath(), "world" );
		_folder = Path.Combine( rootPath, nameof( TectonicLandformBuilderIntegrationTests ) );
		Directory.CreateDirectory( _folder );
		var services = new ServiceCollection();

		services.AddDelaunayVoronoiWorldBuilder();
		services.AddRasterizer();
		services.AddBuffers();

		_provider = services.BuildServiceProvider();
	}

	[SetUp]
	public void SetUp() {
		_scope = _provider.CreateScope();

		_rasterizer = _provider.GetRequiredService<IRasterizer>();
		_bufferFactory = _provider.GetRequiredService<IBufferFactory>();

		IVoronoiBuilder voronoiBuilder = _provider.GetRequiredService<IVoronoiBuilder>();
		ISearchableVoronoiFactory searchableVoronoiFactory = _provider.GetRequiredService<ISearchableVoronoiFactory>();

		_landformBuilder = new TectonicLandformBuilder(
			voronoiBuilder,
			searchableVoronoiFactory
		);
	}

	[TearDown]
	public void TearDown() {
		 _scope.Dispose();
	}

	[OneTimeTearDown]
	public void OneTimeTearDown() {
		Directory.Delete( _folder, true );
	}

	[Test]
	[Ignore( "Used to visualize output for inspection." )]
	public async Task Visualize() {
		ISize size = new Point( 1600, 900 );
		IReadOnlySet<Cell> landform = _landformBuilder.Create( size, out ISearchableVoronoi voronoi );

		IBuffer<float> buffer = _bufferFactory.Create<float>( size );

		foreach( Cell cell in landform ) {
			_rasterizer.Rasterize( cell.Polygon.Points, ( int x, int y ) => {
				buffer[x, y] = 0.3f;
			} );
		}

		foreach( Edge edge in voronoi.Edges ) {
			_rasterizer.Rasterize( edge.A, edge.B, ( int x, int y ) => {
				buffer[x, y] = 0.2f;
			} );
		}

		IBufferWriter<float> writer = new ImageBufferWriter( Path.Combine( _folder, "tectonic_landform.png" ) );
		await writer.WriteAsync( buffer );
	}
}
