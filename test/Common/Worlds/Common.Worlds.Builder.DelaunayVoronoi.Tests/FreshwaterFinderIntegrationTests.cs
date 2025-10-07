using Kiyote.Buffers;
using Kiyote.Geometry;
using Kiyote.Geometry.DelaunayVoronoi;
using Kiyote.Geometry.Rasterizers;

namespace Common.Worlds.Builder.DelaunayVoronoi.Tests;

[TestFixture]
internal sealed class FreshwaterFinderIntegrationTests {

	private ILandformBuilder _landformBuilder;
	private IBufferFactory _bufferFactory;
	private IRasterizer _rasterizer;
	private ISaltwaterFinder _saltwaterBuilder;
	private ITectonicPlateBuilder _tectonicPlateBuilder;
	private NotSaltwaterFreshwaterFinder _builder;

	private IServiceProvider _provider;
	private IServiceScope _scope;
	private string _folder;

	[OneTimeSetUp]
	public void OneTimeSetUp() {
		string rootPath = Path.Combine( Path.GetTempPath(), "world" );
		_folder = Path.Combine( rootPath, nameof( FreshwaterFinderIntegrationTests ) );
		Directory.CreateDirectory( _folder );
		var services = new ServiceCollection();
		services.AddBuffers();
		services.AddCommonWorlds();
		services.AddDelaunayVoronoiWorldBuilder();
		services.AddRandomization();
		services.AddDelaunayVoronoi();
		services.AddRasterizer();

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
		_tectonicPlateBuilder = _scope.ServiceProvider.GetRequiredService<ITectonicPlateBuilder>();

		_builder = new NotSaltwaterFreshwaterFinder();
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
		Landform landform = await _landformBuilder.CreateAsync( size, tectonicPlates, TestContext.CurrentContext.CancellationToken ).ConfigureAwait( false );
		IReadOnlySet<Cell> saltwater = _saltwaterBuilder.Find( size, landform.Map, landform.Cells );
		IReadOnlySet<Cell> lakes = ( _builder as IFreshwaterFinder ).Create( size, landform.Map, landform.Cells, saltwater );

		IBuffer<float> buffer = _bufferFactory.Create<float>( size, 0.0f );

		foreach( Cell cell in landform.Cells ) {
			_rasterizer.Rasterize( cell.Polygon.Points, ( int x, int y ) => {
				buffer[x, y] = 0.3f;
			} );
		}

		foreach( Cell lake in lakes ) {
			_rasterizer.Rasterize( lake.Polygon.Points, ( int x, int y ) => {
				buffer[x, y] = 0.75f;
			} );
		}

		foreach( Edge edge in landform.Map.Edges ) {
			_rasterizer.Rasterize( edge.A, edge.B, ( int x, int y ) => {
				buffer[x, y] = 0.2f;
			} );
		}

		IBufferWriter<float> writer = new ImageBufferWriter( Path.Combine( _folder, "freshwater.png" ) );
		await writer.WriteAsync( buffer );
	}
}
