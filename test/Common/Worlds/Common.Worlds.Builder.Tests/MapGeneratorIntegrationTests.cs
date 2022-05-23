using Common.Buffers;
using Common.Buffers.Float;
using Common.Worlds.Builder.DelaunayVoronoi;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Size = Common.Core.Size;

namespace Common.Worlds.Builder.Tests;

[TestFixture]
public sealed class MapGeneratorIntegrationTests {

	private IRandom _random;
	private ILandformMapGenerator _landformMapGenerator;
	private INeighbourLocator _neighbourLocator;
	private IMapGenerator _mapGenerator;
	private IServiceProvider _provider;
	private IServiceScope _scope;
	private string _folder;

	[OneTimeSetUp]
	public void OneTimeSetUp() {
		string rootPath = Path.Combine( Path.GetTempPath(), "world" );
		Directory.CreateDirectory( rootPath );
		_folder = Path.Combine( rootPath, nameof( MapGeneratorIntegrationTests ) );
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

		_random = _scope.ServiceProvider.GetRequiredService<IRandom>();
		_neighbourLocator = _scope.ServiceProvider.GetRequiredService<INeighbourLocator>();
		_landformMapGenerator = _scope.ServiceProvider.GetRequiredService<ILandformMapGenerator>();
		_mapGenerator = _scope.ServiceProvider.GetRequiredService<IMapGenerator>();
	}

	[TearDown]
	public void TearDown() {
		_scope.Dispose();
	}

	[Test]
	[Ignore("Visualize output for inspection.")]
	public void Visualize() {
		long seed = (_random.NextInt() << 32) | _random.NextInt();
		Size size = new Size( 1000, 1000 );
		LandformMaps landformMaps = _landformMapGenerator.Create(
			seed,
			size,
			_neighbourLocator
		);

		WorldMaps worldMaps = _mapGenerator.Create( landformMaps );

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

