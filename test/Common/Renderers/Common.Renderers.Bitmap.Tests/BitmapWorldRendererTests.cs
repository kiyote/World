using Common.Files;
using Common.Files.Manager.Resource;
using Moq;
using NUnit.Framework;
using Server.Files.Manager.Disk;
using Common.Worlds;

namespace Common.Renderers.Bitmap.Tests;

[TestFixture]
public class BitmapWorldRendererTests {

	private Mock<IResourceFileManager> _resourceFileManager;
	private BitmapWorldRenderer _renderer;

	[SetUp]
	public void SetUp() {
		_resourceFileManager = new Mock<IResourceFileManager>( MockBehavior.Strict );
		_renderer = new BitmapWorldRenderer(
			_resourceFileManager.Object
		);
	}

	[Test]
	[Ignore("Used to generate visual output for inspection.")]
	public async Task RenderTerrain() {
		ResourceFileManager resourceFileManager = ResourceFileManager.GetInstance();
		DiskFileManager diskFileManager = DiskFileManager.GetInstance( @"c:\temp" );
		IWorldRenderer bitmapWorldRenderer = new BitmapWorldRenderer( resourceFileManager );
		Id<FileMetadata> fileId = new Id<FileMetadata>( "terrain.png" );
		TileTerrain[,] tileTerrain = new TileTerrain[10, 10];

		for( int r = 0; r < tileTerrain.GetLength( 0 ); r++ ) {
			for( int c = 0; c < tileTerrain.GetLength( 1 ); c++ ) {
				tileTerrain[c, r] = TileTerrain.Plain;
			}
		}

		await bitmapWorldRenderer.RenderTerrainToAsync(
			diskFileManager,
			fileId,
			tileTerrain,
			CancellationToken.None
		).ConfigureAwait( false );
	}
}
