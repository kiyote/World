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

	[OneTimeSetUp]
	public void OneTimeSetUp() {
		var services = new ServiceCollection();
		services.AddResourceFileManager();
		services.AddDiskFileManager( @"c:\temp" );
		services.AddBitmapRenderer();

		_provider = services.BuildServiceProvider();
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
		TileTerrain[,] tileTerrain = new TileTerrain[10, 10];

		for( int r = 0; r < tileTerrain.GetLength( 0 ); r++ ) {
			for( int c = 0; c < tileTerrain.GetLength( 1 ); c++ ) {
				tileTerrain[c, r] = TileTerrain.Plain;
			}
		}

		await _renderer.RenderTerrainToAsync(
			diskFileManager,
			fileId,
			tileTerrain,
			CancellationToken.None
		).ConfigureAwait( false );
	}
}
