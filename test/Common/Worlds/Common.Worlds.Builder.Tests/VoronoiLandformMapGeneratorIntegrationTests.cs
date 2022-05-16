using Common.Buffers;
using Common.Buffers.Float;
using Common.Geometry;
using Common.Geometry.DelaunayVoronoi;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using TestHelpers;
using Size = Common.Core.Size;

namespace Common.Worlds.Builder.Tests;

#pragma warning disable CA1812
[TestFixture]
internal sealed class VoronoiLandformMapGeneratorIntegrationTests {

	private IRandom _random;
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
		services.AddGeometry();
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

		_random = _provider.GetRequiredService<IRandom>();
		_generator = new VoronoiLandformMapGenerator(
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
			new HexNeighbourLocator()
		);

		IBufferWriter<float> writer = new ImageBufferWriter( Path.Combine( _folder, "heightmap.png" ) );
		await writer.WriteAsync( landform );
	}
}
#pragma warning restore CA1812
