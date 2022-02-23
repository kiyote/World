using Common.Files;
using Common.Files.Manager;
using Common.Files.Manager.Resource;
using Common.Worlds;
using Moq;
using NUnit.Framework;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Common.Renderers.Bitmap.Tests;

[TestFixture]
public class BitmapWorldRendererUnitTests {

	private Mock<IResourceFileManager> _resourceFileManager;
	private Mock<IImageFactory> _imageFactory;
	private IWorldRenderer _renderer;

	[SetUp]
	public void SetUp() {
		_imageFactory = new Mock<IImageFactory>( MockBehavior.Strict );
		_resourceFileManager = new Mock<IResourceFileManager>( MockBehavior.Strict );
		SetupResourceFileManager( _resourceFileManager, _imageFactory );

		_renderer = new BitmapWorldRenderer(
			_resourceFileManager.Object,
			_imageFactory.Object
		);
	}

	[Test]
	[System.Diagnostics.CodeAnalysis.SuppressMessage( "Reliability", "CA2000:Dispose objects before losing scope", Justification = "Will eventually expire with test run." )]
	public async Task RenderToTerrainAsync() {
		Id<FileMetadata> fileId = new Id<FileMetadata>( "test" );
		TileTerrain[,] terrain = new TileTerrain[2, 2];
		for( int r = 0; r < terrain.GetLength(0); r++) {
			for (int c = 0; c < terrain.GetLength(1); c++) {
				terrain[c, r] = TileTerrain.Plain;
			}
		}
		Mock<IMutableFileManager> fileManager = new Mock<IMutableFileManager>( MockBehavior.Strict );

		Image<Argb32> image = new Image<Argb32>( 122, 160 );
		_imageFactory
			.Setup( f => f.Create( 122, 160 ) )
			.Returns( image );

		fileManager
			.Setup( fm => fm.PutContentAsync( fileId, It.IsAny<AsyncStreamHandler>(), It.IsAny<CancellationToken>() ) )
			.Returns( Task.CompletedTask );

		await _renderer.RenderAtlasToAsync(
			fileManager.Object,
			fileId,
			terrain,
			CancellationToken.None
		);
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage( "Reliability", "CA2000:Dispose objects before losing scope", Justification = "Will eventually expire with the test run." )]
	private static void SetupResourceFileManager(
		Mock<IResourceFileManager> resourceFileManager,
		Mock<IImageFactory> imageFactory
	) {
		Image<Argb32> image = new Image<Argb32>( 1, 1 );

		Id<FileMetadata> mountainId = new Id<FileMetadata>( "mountain" );
		Mock<Stream> stream = new Mock<Stream>( MockBehavior.Strict );
		imageFactory
			.Setup( f => f.LoadAsync( stream.Object, It.IsAny<CancellationToken>() ) )
			.Returns( Task.FromResult( image ) );
		resourceFileManager
			.Setup( rfm => rfm.MountainTerrainId )
			.Returns( mountainId );
		resourceFileManager
			.Setup( rfm => rfm.TryGetContentAsync( mountainId, It.IsAny<AsyncStreamHandler>(), It.IsAny<CancellationToken>() ) )
			.Callback<Id<FileMetadata>, AsyncStreamHandler, CancellationToken>( (fileId, func, token) => {
				func.Invoke( stream.Object ).Wait( token );
			} )
			.Returns( Task.FromResult( true ) );

		Id<FileMetadata> hillId = new Id<FileMetadata>( "hill" );
		resourceFileManager
			.Setup( rfm => rfm.HillTerrainId )
			.Returns( hillId );
		resourceFileManager
			.Setup( rfm => rfm.TryGetContentAsync( hillId, It.IsAny<AsyncStreamHandler>(), It.IsAny<CancellationToken>() ) )
			.Callback<Id<FileMetadata>, AsyncStreamHandler, CancellationToken>( ( fileId, func, token ) => {
				func.Invoke( stream.Object ).Wait( token );
			} )
			.Returns( Task.FromResult( true ) );

		Id<FileMetadata> plainsId = new Id<FileMetadata>( "plains" );
		resourceFileManager
			.Setup( rfm => rfm.PlainsTerrainId )
			.Returns( plainsId );
		resourceFileManager
			.Setup( rfm => rfm.TryGetContentAsync( plainsId, It.IsAny<AsyncStreamHandler>(), It.IsAny<CancellationToken>() ) )
			.Callback<Id<FileMetadata>, AsyncStreamHandler, CancellationToken>( ( fileId, func, token ) => {
				func.Invoke( stream.Object ).Wait( token );
			} )
			.Returns( Task.FromResult( true ) );
	}
}
