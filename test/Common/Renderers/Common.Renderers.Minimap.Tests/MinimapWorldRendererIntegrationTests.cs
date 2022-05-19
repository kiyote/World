using Common.Buffers;
using Common.Files;
using Common.Files.Manager.Resource;
using Common.Worlds;
using Common.Worlds.Builder;
using Server.Files.Manager.Disk;

namespace Common.Renderers.Minimap.Tests;

[TestFixture]
public class MinimapWorldRendererIntegrationTests {
	/*

	private IServiceScope _scope;
	private IServiceProvider _provider;
	private IWorldRenderer _renderer;
	private string _folder;

	[OneTimeSetUp]
	public void OneTimeSetUp() {
		string testRoot = Path.Combine( Path.GetTempPath(), "world" );
		Directory.CreateDirectory( testRoot );
		_folder = Path.Combine( testRoot, Guid.NewGuid().ToString( "N" ) );
		Directory.CreateDirectory( _folder );

		var services = new ServiceCollection();
		services.AddCore();
		services.AddResourceFileManager();
		services.AddDiskFileManager( _folder );
		services.AddMinimapRenderer();
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

		_renderer = _scope.ServiceProvider.GetRequiredService<IWorldRenderer>();
	}

	[TearDown]
	public void TearDown() {
		_scope.Dispose();
		_scope = null;
	}

	[Test]
	[Ignore( "Used to generate visual output for inspection." )]
	public async Task RenderTerrain() {
		IDiskFileManager diskFileManager = _provider.GetRequiredService<IDiskFileManager>();
		ITerrainMapGenerator mapGenerator = _provider.GetRequiredService<ITerrainMapGenerator>();
		Size size = new Size( 1000, 1000 );
		IBuffer<TileTerrain> tileTerrain = mapGenerator.GenerateTerrain( Hash.GetLong( "test" ), size );

		Id<FileMetadata> fileId = new Id<FileMetadata>( "terrain.png" );

		await _renderer.RenderAtlasToAsync(
			diskFileManager,
			fileId,
			tileTerrain,
			CancellationToken.None
		).ConfigureAwait( false );
	}
	*/
}
