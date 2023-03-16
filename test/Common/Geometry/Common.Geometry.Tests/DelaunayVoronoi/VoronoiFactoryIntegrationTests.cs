using Common.Buffers;
using Size = Common.Core.Size;

namespace Common.Geometry.DelaunayVoronoi.Tests;

internal sealed class VoronoiIntegrationTests {

	private IRandom _random;
	private IGeometry _geometry;
	private IBufferFactory _bufferFactory;
	private IPointFactory _pointFactory;
	private IDelaunatorFactory _delaunatorFactory;
	private IVoronoiFactory _voronoiFactory;

	private IServiceProvider _provider;
	private IServiceScope _scope;
	private string _folder;

	[OneTimeSetUp]
	public void OneTimeSetUp() {
		string rootPath = Path.Combine( Path.GetTempPath(), "world" );
		_folder = Path.Combine( rootPath, nameof( VoronoiIntegrationTests ) );
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
		_voronoiFactory = _scope.ServiceProvider.GetRequiredService<IVoronoiFactory>();
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
		IVoronoi voronoi = _voronoiFactory.Create( delaunator, size.Columns, size.Rows );

		foreach( Cell cell in voronoi.Cells.Where( c => !c.IsOpen ) ) {
			_geometry.RasterizePolygon(
				cell.Points,
				( int x, int y ) => {
					result[x, y] = 0.5f;
				}
			);
		}

		IBufferWriter<float> writer = new ImageBufferWriter( Path.Combine( _folder, "voronoi.png" ) );
		await writer.WriteAsync( result );

		result = _bufferFactory.Create<float>( size.Columns, size.Rows );
		Cell start = voronoi.Cells.Where( c => !c.IsOpen ).First();
		Queue<Cell> queue = new Queue<Cell>();
		HashSet<Cell> visited = new HashSet<Cell>();
		queue.Enqueue( start );
		while( queue.Any() ) {
			Cell cell = queue.Dequeue();
			visited.Add( cell );
			foreach( Cell neighbour in voronoi.Neighbours[cell].Where( c => !c.IsOpen ) ) {
				if( !visited.Contains( neighbour ) && !queue.Contains( neighbour ) ) {
					queue.Enqueue( neighbour );
				}
			}
			_geometry.RasterizePolygon(
				cell.Points,
				( int x, int y ) => {
					result[x, y] = 0.5f;
				}
			);

		}
		writer = new ImageBufferWriter( Path.Combine( _folder, "voronoi_neighbours.png" ) );
		await writer.WriteAsync( result );
	}
}
