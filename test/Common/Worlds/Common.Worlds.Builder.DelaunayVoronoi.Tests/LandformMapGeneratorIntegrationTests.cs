using Common.Buffers;
using Common.Buffers.Float;
using Common.Geometry;
using Common.Geometry.DelaunayVoronoi;
using Size = Common.Core.Size;

namespace Common.Worlds.Builder.DelaunayVoronoi.Tests;

[TestFixture]
internal sealed class LandformMapGeneratorIntegrationTests {

	private IRandom _random;
	private INeighbourLocator _neighbourLocator;

	private ILandformMapGenerator _generator;
	private IServiceProvider _provider;
	private IServiceScope _scope;
	private string _folder;

	[OneTimeSetUp]
	public void OneTimeSetUp() {
		string rootPath = Path.Combine( Path.GetTempPath(), "world" );
		_folder = Path.Combine( rootPath, nameof( LandformMapGeneratorIntegrationTests ) );
		Directory.CreateDirectory( _folder );
		var services = new ServiceCollection();
		services.AddCore();
		services.AddBuffers();
		services.AddFloatBuffers();
		services.AddCoreWorldBuilder();
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
			_scope.ServiceProvider.GetRequiredService<IBufferOperator>(),
			_scope.ServiceProvider.GetRequiredService<IFloatBufferOperators>(),
			_scope.ServiceProvider.GetRequiredService<IBufferFactory>(),
			_scope.ServiceProvider.GetRequiredService<IGeometry>(),
			_scope.ServiceProvider.GetRequiredService<IMountainsBuilder>(),
			_scope.ServiceProvider.GetRequiredService<ILandformBuilder>(),
			_scope.ServiceProvider.GetRequiredService<IHillsBuilder>(),
			_scope.ServiceProvider.GetRequiredService<ISaltwaterBuilder>(),
			_scope.ServiceProvider.GetRequiredService<IFreshwaterBuilder>()
		);
	}

	[TearDown]
	public void TearDown() {
		_scope.Dispose();
	}

	[Test]
	[Ignore( "Used to visualize output for inspection." )]
	public async Task Visualize() {
		Size size = new Size( 1000, 1000 );
		LandformMaps landformMaps = _generator.Create(
			_random.NextInt( int.MaxValue ),
			size,
			_neighbourLocator
		);

		IBufferWriter<float> floatWriter = new ImageBufferWriter( Path.Combine( _folder, "height.png" ) );
		await floatWriter.WriteAsync( landformMaps.Height );

		floatWriter = new ImageBufferWriter( Path.Combine( _folder, "temperature.png" ) );
		await floatWriter.WriteAsync( landformMaps.Temperature );

		floatWriter = new ImageBufferWriter( Path.Combine( _folder, "moisture.png" ) );
		await floatWriter.WriteAsync( landformMaps.Moisture );

		IBufferWriter<bool> boolWriter = new ImageBufferWriter( Path.Combine( _folder, "freshwater.png" ) );
		await boolWriter.WriteAsync( landformMaps.FreshWater );

		boolWriter = new ImageBufferWriter( Path.Combine( _folder, "saltwater.png" ) );
		await boolWriter.WriteAsync( landformMaps.SaltWater );
	}
}
