using Common.Buffers;
using Common.Buffers.Float;
using Common.Geometry;
using Common.Geometry.DelaunayVoronoi;
using Size = Common.Core.Size;

namespace Common.Worlds.Builder.DelaunayVoronoi.Tests;

#pragma warning disable CA1812
[TestFixture]
internal sealed class VoronoiLandformMapGeneratorIntegrationTests {

	private IRandom _random;
	private INeighbourLocator _neighbourLocator;

	private ILandformMapGenerator _generator;
	private IServiceProvider _provider;
	private IServiceScope _scope;
	private string _folder;

	[OneTimeSetUp]
	public void OneTimeSetUp() {
		string rootPath = Path.Combine( Path.GetTempPath(), "world" );
		_folder = Path.Combine( rootPath, nameof( VoronoiLandformMapGeneratorIntegrationTests ) );
		Directory.CreateDirectory( _folder );
		var services = new ServiceCollection();
		services.AddCore();
		services.AddFloatBufferOperators();
		services.AddArrayBuffer();
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

		_random = _provider.GetRequiredService<IRandom>();
		_neighbourLocator = _provider.GetRequiredService<INeighbourLocator>();

		_generator = new LandformMapGenerator(
			_scope.ServiceProvider.GetRequiredService<IRandom>(),
			_scope.ServiceProvider.GetRequiredService<IDelaunatorFactory>(),
			_scope.ServiceProvider.GetRequiredService<IVoronoiFactory>(),
			_scope.ServiceProvider.GetRequiredService<IBufferFactory>(),
			_scope.ServiceProvider.GetRequiredService<IGeometry>(),
			_scope.ServiceProvider.GetRequiredService<IMountainRangeBuilder>()
		);
	}

	[TearDown]
	public void TearDown() {
		_scope.Dispose();
	}

	[Test]
	[Ignore("Used to visualize output for inspection.")]
	public async Task Visualize() {
		Size size = new Size( 1000, 1000 );
		IBuffer<float> landform = _generator.Create(
			_random.NextInt( int.MaxValue ),
			size,
			_neighbourLocator
		);

		IBufferWriter<float> writer = new ImageBufferWriter( Path.Combine( _folder, "heightmap.png" ) );
		await writer.WriteAsync( landform );
	}
}
#pragma warning restore CA1812
