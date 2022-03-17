using Common.Files;
using Common.Files.Manager.Resource;
using Common.Worlds;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Server.Files.Manager.Disk;

namespace Common.Renderers.Bitmap.Tests;

[TestFixture]
public class BitmapWorldRendererIntegrationTests {

	private IServiceScope _scope;
	private IServiceProvider _provider;
	private IWorldRenderer _renderer;
	private string _folder;

	[OneTimeSetUp]
	public void OneTimeSetUp() {
		_folder = Path.Combine( Path.GetTempPath(), Guid.NewGuid().ToString( "N" ) );
		Directory.CreateDirectory( _folder );

		var services = new ServiceCollection();
		services.AddResourceFileManager();
		services.AddDiskFileManager( _folder );
		services.AddBitmapRenderer();

		_provider = services.BuildServiceProvider();
	}

	[OneTimeTearDown]
	public void OneTimeTearDown() {
		Directory.Delete( _folder, true );
	}

	[SetUp]
	public void SetUp() {
		_scope = _provider.CreateScope();

		_renderer = _provider.GetRequiredService<IWorldRenderer>();
	}

	[TearDown]
	public void TearDown() {
		_scope.Dispose();
	}

	[Test]
	[Ignore("Used to generate visual output for inspection.")]
	public async Task RenderTerrain() {
		IDiskFileManager diskFileManager = _provider.GetRequiredService<IDiskFileManager>();

		Id<FileMetadata> fileId = new Id<FileMetadata>( "terrain.png" );
		Buffer<TileTerrain> tileTerrain = new Buffer<TileTerrain>( 10, 10 );

		for( int r = 0; r < tileTerrain.Size.Rows; r++ ) {
			for( int c = 0; c < tileTerrain.Size.Columns; c++ ) {
				tileTerrain[r][c] = TileTerrain.Grass;
			}
		}

		await _renderer.RenderAtlasToAsync(
			diskFileManager,
			fileId,
			tileTerrain,
			CancellationToken.None
		).ConfigureAwait( false );
	}
}
