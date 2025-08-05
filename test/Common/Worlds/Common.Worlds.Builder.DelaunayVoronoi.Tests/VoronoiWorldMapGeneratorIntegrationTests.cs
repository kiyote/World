using Kiyote.Buffers.Float;
using Kiyote.Geometry;
using Kiyote.Geometry.Randomization;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Point = Kiyote.Geometry.Point;

namespace Common.Worlds.Builder.DelaunayVoronoi.Tests;

[TestFixture]
internal sealed class VoronoiWorldMapGeneratorIntegrationTests {

	private IRandom _random;
	private INeighbourLocator _neighbourLocator;
	private IWorldMapGenerator _worldMapGenerator;

	private IServiceProvider _provider;
	private IServiceScope _scope;
	private string _folder;

	[OneTimeSetUp]
	public void OneTimeSetUp() {
		string rootPath = Path.Combine( Path.GetTempPath(), "world" );
		_folder = Path.Combine( rootPath, nameof( VoronoiWorldMapGeneratorIntegrationTests ) );
		Directory.CreateDirectory( _folder );
		var services = new ServiceCollection();
		services.AddCommonWorlds();
		services.AddFloatBuffers();
		services.AddDelaunayVoronoiWorldBuilder();
		services.AddRandomization();
		services.AddRasterizer();
		services.AddDelaunayVoronoi();

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

		_worldMapGenerator = _scope.ServiceProvider.GetRequiredService<IWorldMapGenerator>();
	}

	[TearDown]
	public void TearDown() {
		_scope.Dispose();
	}

	[Test]
	//[Ignore( "Used to visualize output for inspection." )]
	public void Visualize() {
		long seed = DateTime.UtcNow.Ticks;
		ISize size = new Point( 1600, 900 );
		WorldMaps worldMaps = _worldMapGenerator.Create(
			seed,
			size,
			_neighbourLocator
		);

		using Image<Rgba32> image = new Image<Rgba32>( size.Width, size.Height );

		Dictionary<TileTerrain, Rgba32> terrainColours = new Dictionary<TileTerrain, Rgba32> {
			{ TileTerrain.Mountain, new Rgba32( 0xF7, 0xF7, 0xF7 ) },
			{ TileTerrain.Hill, new Rgba32( 0xDC, 0xDD, 0xBE ) },
			{ TileTerrain.Lake, new Rgba32( 0x6E, 0xBA, 0xE7 ) },
			{ TileTerrain.Plain, new Rgba32( 0xB7, 0xC1, 0x8C ) },
			{ TileTerrain.Coast, new Rgba32( 0x6E, 0xBA, 0xE7 ) },
			{ TileTerrain.Ocean, new Rgba32( 0x1C, 0x86, 0xEE ) }
		};

		Dictionary<TileFeature, Rgba32> featureColours = new Dictionary<TileFeature, Rgba32> {
			{ TileFeature.Tundra, new Rgba32( 0xE0, 0xE0, 0xE0 ) },
			{ TileFeature.RockyDesert, new Rgba32( 0xC6, 0xCC, 0xAD ) },
			{ TileFeature.SandyDesert, new Rgba32( 0xFC, 0xE7, 0x92 ) },
			{ TileFeature.BorealForest, new Rgba32( 0x6C, 0xB2, 0x76 ) },
			{ TileFeature.TemperateForest, new Rgba32( 0x39, 0xB5, 0x4A ) },
			{ TileFeature.TropicalForest, new Rgba32( 0x0E, 0xAF, 0x20 ) }
		};

		for( int r = 0; r < size.Height; r++ ) {
			for( int c = 0; c < size.Width; c++ ) {
				Rgba32 colour = terrainColours[worldMaps.Terrain[c, r]];
				if( worldMaps.Feature[c, r] != TileFeature.None ) {
					featureColours.TryGetValue( worldMaps.Feature[c, r], out colour );
					//colour = featureColours[worldMaps.Feature[c, r]];
				}
				image[c, r] = colour;
			}
		}

		image.Save( Path.Combine( _folder, "worldmap.png" ) );
	}
}
