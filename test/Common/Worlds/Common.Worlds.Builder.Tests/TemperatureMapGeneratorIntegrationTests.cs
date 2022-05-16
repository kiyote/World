using Common.Buffers;
using Common.Buffers.Float;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using TestHelpers;

namespace Common.Worlds.Builder.Tests;

[TestFixture]
internal sealed class TemperatureMapGeneratorIntegrationTests {

	private INeighbourLocator _neighbourLocator;
	private ILandformMapGenerator _landformGenerator;
	private ITemperatureMapGenerator _generator;
	private IServiceProvider _provider;
	private IServiceScope _scope;
	private string _folder;

	[OneTimeSetUp]
	public void OneTimeSetUp() {
		string rootPath = Path.Combine( Path.GetTempPath(), "world" );
		Directory.CreateDirectory( rootPath );
		_folder = Path.Combine( rootPath, nameof( TemperatureMapGeneratorIntegrationTests ) );
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

		_generator = new TemperatureMapGenerator(
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

		IBuffer<float> temperature = _generator.Create( landform );

		IBufferWriter<float> writer = new ImageBufferWriter( Path.Combine( _folder, "temperature.png" ) );
		await writer.WriteAsync( temperature );
	}
}
