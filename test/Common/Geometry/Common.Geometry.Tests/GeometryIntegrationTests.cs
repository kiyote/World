using Common.Buffers;

namespace Common.Geometry.Tests;

public class GeometryIntegrationTests {

	private IRandom _random;
	private IGeometry _geometry;
	private IBufferFactory _bufferFactory;
	private IServiceProvider _provider;
	private IServiceScope _scope;
	private string _folder;

	[OneTimeSetUp]
	public void OneTimeSetUp() {
		string rootPath = Path.Combine( Path.GetTempPath(), "world" );
		_folder = Path.Combine( rootPath, nameof( GeometryIntegrationTests ) );
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
		_bufferFactory = _scope.ServiceProvider.GetRequiredService<IBufferFactory>();
	}

	[TearDown]
	public void TearDown() {
		_scope.Dispose();
	}

	[OneTimeTearDown]
	public void OneTimeTearDown() {
		Directory.Delete( _folder, true );
	}

	[TestCaseSource( nameof( LineLengthTestCaseSource ) )]
	public void LineLength(
		Point p1,
		Point p2,
		int expected
	) {
		Assert.AreEqual( expected, _geometry.LineLength( p1, p2 ) );
	}

	private static IEnumerable<object> LineLengthTestCaseSource() {
		yield return new object[] { new Point( 0, 100 ), new Point( 100, 100 ), 100 };
		yield return new object[] { new Point( 0, 0 ), new Point( 100, 0 ), 100 };
		yield return new object[] { new Point( 100, 100 ), new Point( 0, 100 ), 100 };
		yield return new object[] { new Point( 100, 0 ), new Point( 0, 0 ), 100 };
		yield return new object[] { new Point( 0, 0 ), new Point( 100, 100 ), 141 };
	}

	[Test]
	[Ignore( "Used to visualize output for inspection." )]
	public async Task Visualize_PointInPolygon() {
		IBuffer<float> result = _bufferFactory.Create<float>( 500, 500 );

		/* Counter-clockwise		*/
		List<Point> points = new List<Point>() {
			new Point( 100, 100 ),
			new Point( 0, 200 ),
			new Point( 75, 300 ),
			new Point( 133, 400 ),
			new Point( 250, 500 ),
			new Point( 400, 400 ),
			new Point( 500, 300 ),
			new Point( 425, 250 ),
			new Point( 300, 100 ),
			new Point( 100, 100 )
		};

		for( int i = 0; i < 8000; i++ ) {
			Point p = new Point(
				_random.NextInt( 500 ),
				_random.NextInt( 500 )
			);


			if( _geometry.PointInPolygon( points, p ) ) {
				result[p.X, p.Y] = 1.0f;
			} else {
				result[p.X, p.Y] = 0.5f;
			}
		}

		IBufferWriter<float> writer = new ImageBufferWriter( Path.Combine( _folder, "pointinpolygon.png" ) );
		await writer.WriteAsync( result );
	}

	[Test]
	[Ignore( "Used to visualize output for inspection." )]
	public async Task Visualize_RasterizeLine() {
		IBuffer<float> result = _bufferFactory.Create<float>( 500, 500 );

		Point p1 = new Point( 150, 100 );
		Point p2 = new Point( 350, 400 );

		_geometry.RasterizeLine( p1, p2, ( int x, int y ) => {
			result[x, y] = 1.0f;
		} );

		IBufferWriter<float> writer = new ImageBufferWriter( Path.Combine( _folder, "rasterizeline.png" ) );
		await writer.WriteAsync( result );
	}

	[Test]
	[Ignore( "Used to visualize output for inspection." )]
	public async Task Visualize_RasterizePolygon() {
		IBuffer<float> result = _bufferFactory.Create<float>( 500, 500 );

		List<Point> polygon = new List<Point>() {
			new Point( 250, 50 ),
			new Point( 150, 100 ),
			new Point( 50, 250 ),
			new Point( 150, 400 ),
			new Point( 250, 450 ),
			new Point( 350, 400 ),
			new Point( 450, 250 ),
			new Point( 350, 100 )
		};

		_geometry.RasterizePolygon( polygon, ( int x, int y ) => {
			result[x, y] = 1.0f;
		} );

		IBufferWriter<float> writer = new ImageBufferWriter( Path.Combine( _folder, "rasterizepolygon.png" ) );
		await writer.WriteAsync( result );
	}

}
