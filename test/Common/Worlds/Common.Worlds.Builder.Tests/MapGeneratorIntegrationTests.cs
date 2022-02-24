using Common.Files;
using Common.Files.Manager.Resource;
using Common.Renderers;
using Common.Renderers.Bitmap;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Server.Files.Manager.Disk;

namespace Common.Worlds.Builder.Tests;

[TestFixture]
public sealed class MapGeneratorIntegrationTests {

	private IMapGenerator _mapGenerator;
	private IServiceProvider _provider;
	private IServiceScope _scope;
	private string _folder;

	[OneTimeSetUp]
	public void OneTimeSetUp() {
		_folder = Path.Combine( Path.GetTempPath(), Guid.NewGuid().ToString( "N" ) );
		Directory.CreateDirectory( _folder );
		var services = new ServiceCollection();
		services.AddCore();
		services.AddWorldBuilder();
		services.AddBitmapRenderer();
		services.AddDiskFileManager( _folder );
		services.AddResourceFileManager();

		_provider = services.BuildServiceProvider();
	}

	[OneTimeTearDown]
	public void OneTimeTearDown() {
		Directory.Delete( _folder, true );
	}

	[SetUp]
	public void SetUp() {
		_scope = _provider.CreateScope();

		_mapGenerator = _provider.GetRequiredService<IMapGenerator>();
	}

	[TearDown]
	public void TearDown() {
		_scope.Dispose();
	}

	[Test]
	[Ignore("Used to generate visual output for inspection.")]
	public void GenerateImage() {
		Id<FileMetadata> fileId = new Id<FileMetadata>( "terrain.png" );
		TileTerrain[,] terrain = _mapGenerator.GenerateTerrain( "test", 100, 100 );
		IWorldRenderer worldRenderer = _provider.GetRequiredService<IWorldRenderer>();
		IDiskFileManager diskFileManager = _provider.GetRequiredService<IDiskFileManager>();
		worldRenderer.RenderAtlasToAsync(
			diskFileManager,
			fileId,
			terrain,
			CancellationToken.None
		);

		worldRenderer.RenderTerrainMapToAsync(
			diskFileManager,
			new Id<FileMetadata>( "terrain_mountain.png"),
			terrain,
			TileTerrain.Mountain,
			CancellationToken.None
		);

		worldRenderer.RenderTerrainMapToAsync(
			diskFileManager,
			new Id<FileMetadata>( "terrain_hill.png" ),
			terrain,
			TileTerrain.Hill,
			CancellationToken.None
		);

		worldRenderer.RenderTerrainMapToAsync(
			diskFileManager,
			new Id<FileMetadata>( "terrain_grass.png" ),
			terrain,
			TileTerrain.Grass,
			CancellationToken.None
		);

		worldRenderer.RenderTerrainMapToAsync(
			diskFileManager,
			new Id<FileMetadata>( "terrain_coast.png" ),
			terrain,
			TileTerrain.Coast,
			CancellationToken.None
		);

		worldRenderer.RenderTerrainMapToAsync(
			diskFileManager,
			new Id<FileMetadata>( "terrain_ocean.png" ),
			terrain,
			TileTerrain.Ocean,
			CancellationToken.None
		);
	}
}

