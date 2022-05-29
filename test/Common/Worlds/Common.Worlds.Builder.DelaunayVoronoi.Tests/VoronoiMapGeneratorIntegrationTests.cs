using Common.Buffers;
using Common.Buffers.Float;
using Common.Geometry;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Size = Common.Core.Size;

namespace Common.Worlds.Builder.DelaunayVoronoi.Tests;

[TestFixture]
internal sealed class VoronoiMapGeneratorIntegrationTests {

	private IRandom _random;
	private INeighbourLocator _neighbourLocator;
	private IWorldMapGenerator _worldMapGenerator;

	private IServiceProvider _provider;
	private IServiceScope _scope;
	private string _folder;

	[OneTimeSetUp]
	public void OneTimeSetUp() {
		string rootPath = Path.Combine( Path.GetTempPath(), "world" );
		_folder = Path.Combine( rootPath, nameof( VoronoiMapGeneratorIntegrationTests ) );
		Directory.CreateDirectory( _folder );
		var services = new ServiceCollection();
		services.AddCommonCore();
		services.AddCommonGeometry();
		services.AddCommonBuffers();
		services.AddCommonWorlds();

		services.AddCommonBuffersFloat();
		services.AddCommonWorldsBuilderDelaunayVoronoi();

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

		_worldMapGenerator = new VoronoiWorldMapGenerator(
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
	public void Visualize() {
		long seed = (long)(_random.NextInt() << 32) | (long)_random.NextInt();
		Size size = new Size( 1000, 1000 );
		WorldMaps worldMaps = _worldMapGenerator.Create(
			seed,
			size,
			_neighbourLocator
		);

		using Image<Rgba32> image = new Image<Rgba32>( size.Columns, size.Rows );

		for (int r = 0; r < size.Rows; r++) {
			for (int c = 0; c < size.Columns; c++) {
				image[c, r] = worldMaps.Terrain[c, r] switch {
					TileTerrain.Mountain => new Rgba32( 0xF7, 0xF7, 0xF7 ),
					TileTerrain.Hill => new Rgba32( 0xDC, 0xDD, 0xBE ),
					TileTerrain.Plain => new Rgba32( 0xB7, 0xC1, 0x8C ),
					TileTerrain.Lake => new Rgba32( 0x6E, 0xBA, 0xE7 ),
					TileTerrain.Coast => new Rgba32( 0x6E, 0xBA, 0xE7 ),
					TileTerrain.Ocean => new Rgba32( 0x1C, 0x86, 0xEE ),
					_ => Color.Black
				};
			}
		}

		for (int r = 0; r < size.Rows; r++) {
			for (int c = 0; c < size.Columns; c++) {
				if( (worldMaps.Feature[c, r] & TileFeature.Forest) == TileFeature.Forest ) {
					image[c, r] = new Rgba32( 0x39, 0xB5, 0x4A );
				}
			}
		}

		image.Save( Path.Combine( _folder, "terrain.png" ) );
	}
}
