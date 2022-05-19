using Common.Buffers;
using Common.Buffers.Float;

namespace Common.Worlds.Builder.Tests;

[TestFixture]
public sealed class AirFlowMapGeneratorIntegrationTests {
	private INeighbourLocator _neighbourLocator;
	private ILandformMapGenerator _landformGenerator;
	private IAirFlowMapGenerator _generator;
	private IServiceProvider _provider;
	private IServiceScope _scope;
	private string _folder;

	[OneTimeSetUp]
	public void OneTimeSetUp() {
		string rootPath = Path.Combine( Path.GetTempPath(), "world" );
		_folder = Path.Combine( rootPath, nameof( AirFlowMapGeneratorIntegrationTests ) );
		Directory.CreateDirectory( _folder );
		var services = new ServiceCollection();
		services.AddCore();
		services.AddFloatBufferOperators();
		services.AddArrayBuffer();
		services.AddWorldBuilder();

		_provider = services.BuildServiceProvider();
	}

	[OneTimeTearDown]
	public void OneTimeTearDown() {
		Directory.Delete( _folder, true );
	}

	[SetUp]
	public void SetUp() {
		_scope = _provider.CreateScope();

		_landformGenerator = _scope.ServiceProvider.GetRequiredService<ILandformMapGenerator>();
		_neighbourLocator = _scope.ServiceProvider.GetRequiredService<INeighbourLocator>();

		_generator = new AirFlowMapGenerator(
			_scope.ServiceProvider.GetRequiredService<IRandom>(),
			_scope.ServiceProvider.GetRequiredService<IBufferFactory>(),
			_scope.ServiceProvider.GetRequiredService<IFloatBufferOperators>()
		);
	}

	[TearDown]
	public void TearDown() {
		_scope.Dispose();
	}

	[Test]
	[Ignore("Used to generate visual output for inspection.")]
	public async Task Visualize() {
		int width = 1000;
		int height = 1000;

		IBuffer<float> landform = _landformGenerator.Create(
			123L,
			new Size( width, height ),
			_neighbourLocator
		);

		IBuffer<float> airflow = _generator.Create( landform );

		IBufferWriter<float> writer = new ImageBufferWriter( Path.Combine( _folder, "airflow.png" ) );
		await writer.WriteAsync( airflow );
	}
}
