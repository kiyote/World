using Common.Buffers;
using Size = Common.Core.Size;

namespace Common.Geometry.DelaunayVoronoi.Tests;

internal sealed class DelaunayIntegrationTests {

	private IRandom _random;
	private IGeometry _geometry;
	private IPointFactory _pointFactory;
	private IBufferFactory _bufferFactory;
	private IDelaunatorFactory _delaunatorFactory;
	private IDelaunayFactory _delaunayFactory;

	private IServiceProvider _provider;
	private IServiceScope _scope;
	private string _folder;

	[OneTimeSetUp]
	public void OneTimeSetUp() {
		string rootPath = Path.Combine( Path.GetTempPath(), "world" );
		_folder = Path.Combine( rootPath, nameof( DelaunayIntegrationTests ) );
		Directory.CreateDirectory( _folder );
		var services = new ServiceCollection();
		services.AddCommonCore();
		services.AddCommonGeometry();
		services.AddCommonBuffers();

		_provider = services.BuildServiceProvider();
	}

	[SetUp]
	public void SetUp() {
		_scope = _provider.CreateScope();

		_random = _scope.ServiceProvider.GetRequiredService<IRandom>();
		_geometry = _scope.ServiceProvider.GetRequiredService<IGeometry>();
		_pointFactory = _scope.ServiceProvider.GetRequiredService<IPointFactory>();
		_bufferFactory = _scope.ServiceProvider.GetRequiredService<IBufferFactory>();
		_delaunatorFactory = _scope.ServiceProvider.GetRequiredService<IDelaunatorFactory>();
		_delaunayFactory = _scope.ServiceProvider.GetRequiredService<IDelaunayFactory>();
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
		Size size = new Size( 500, 500 );
		IBuffer<float> result = _bufferFactory.Create<float>( size.Columns, size.Rows );

		IReadOnlyList<IPoint> points = _pointFactory.Random( 1000, size, 1 );

		Delaunator delaunator = _delaunatorFactory.Create( points );
		Delaunay delaunay = _delaunayFactory.Create( delaunator );

		foreach (Triangle triangle in delaunay.Triangles) {
			_geometry.RasterizePolygon(
				new Point[] { triangle.P1, triangle.P2, triangle.P3 },
				( int x, int y ) => {
					result[x, y] = 0.5f;
				}
			);
		}

		IBufferWriter<float> writer = new ImageBufferWriter( Path.Combine( _folder, "delauanay.png" ) );
		await writer.WriteAsync( result );
	}
}
