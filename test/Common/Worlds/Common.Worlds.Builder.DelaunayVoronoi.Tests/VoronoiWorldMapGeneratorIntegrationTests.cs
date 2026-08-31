using Kiyote.Buffers;
using Kiyote.Buffers.Numerics;
using Kiyote.Geometry;
using Kiyote.Geometry.Noises;
using Kiyote.Geometry.Rasterizers;
using Kiyote.Imaging;
using Kiyote.Imaging.Png;

namespace Common.Worlds.Builder.DelaunayVoronoi.Tests;

[TestFixture]
internal sealed class VoronoiWorldMapGeneratorIntegrationTests {

	private INeighbourLocator _neighbourLocator;
	private IWorldMapGenerator _worldMapGenerator;
	private IBufferFactory _bufferFactory;
	private IImageWriter _imageWriter;

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
		services.AddBuffers();
		services.AddNumericBuffers();
		services.AddDelaunayVoronoiWorldBuilder();
		services.AddRandomization();
		services.AddRasterizer();
		services.AddDelaunayVoronoi();
		services.AddNoise();
		services.AddPngImaging();

		_provider = services.BuildServiceProvider();

	}

	[OneTimeTearDown]
	public void OneTimeTearDown() {
		Directory.Delete( _folder, true );
	}

	[SetUp]
	public void SetUp() {
		_scope = _provider.CreateScope();

		_neighbourLocator = _provider.GetRequiredService<INeighbourLocator>();
		_bufferFactory = _provider.GetRequiredService<IBufferFactory>();
		_imageWriter = _provider.GetRequiredService<IImageWriter>();

		_worldMapGenerator = new VoronoiWorldMapGenerator(
			_bufferFactory,
			_provider.GetRequiredService<IRasterizer>(),
			_provider.GetRequiredService<ILandformBuilder>(),
			_provider.GetRequiredService<ISaltwaterFinder>(),
			_provider.GetRequiredService<IFreshwaterFinder>(),
			_provider.GetRequiredService<ILakeFinder>(),
			_provider.GetRequiredService<ICoastFinder>(),
			_provider.GetRequiredService<ITectonicPlateBuilder>(),
			_provider.GetRequiredService<IInlandDistanceFinder>(),
			_provider.GetRequiredService<IElevationBuilder>(),
			_provider.GetRequiredService<IElevationScaler>()
		);
	}

	[TearDown]
	public void TearDown() {
		_scope.Dispose();
	}

	[Test]
	[Ignore( "Used to visualize output for inspection." )]
	public async Task Visualize() {
		long seed = DateTime.UtcNow.Ticks;
		ISize size = new Point( 1920, 1080 );
		WorldMaps worldMaps = await _worldMapGenerator.CreateAsync(
			seed,
			size,
			_neighbourLocator,
			TestContext.CurrentContext.CancellationToken
		);

		IBuffer<uint> image = _bufferFactory.Create<uint>(size.Width, size.Height, 0 );

		Dictionary<TileTerrain, uint> terrainColours = new Dictionary<TileTerrain, uint> {
			{ TileTerrain.Mountain, 0xF7F7F7FF },
			{ TileTerrain.Hill, 0xDCDDBEFF },
			{ TileTerrain.Highland, 0xCBD3A9FF },
			{ TileTerrain.Lake, 0x6EBae7FF },
			{ TileTerrain.Plain, 0xB7C18CFF },
			{ TileTerrain.Coast, 0x6EBae7FF },
			{ TileTerrain.Ocean, 0x1C86EEFF }
		};

		Dictionary<TileFeature, uint> featureColours = new Dictionary<TileFeature, uint> {
			{ TileFeature.Tundra, 0xE0E0E0FF },
			{ TileFeature.RockyDesert, 0xC6CCADFF },
			{ TileFeature.SandyDesert, 0xFCE792FF },
			{ TileFeature.BorealForest, 0x6CB276FF },
			{ TileFeature.TemperateForest, 0x39B54AFF },
			{ TileFeature.TropicalForest, 0x0EAF20FF }
		};

		for( int r = 0; r < size.Height; r++ ) {
			for( int c = 0; c < size.Width; c++ ) {
				uint colour = terrainColours[worldMaps.Terrain[c, r]];
				if( worldMaps.Feature[c, r] != TileFeature.None ) {
					featureColours.TryGetValue( worldMaps.Feature[c, r], out colour );
					//colour = featureColours[worldMaps.Feature[c, r]];
				}
				image[c, r] = colour;
			}
		}

		_imageWriter.WriteImage( Path.Combine( _folder, "worldmap.png" ), image );
	}
}
