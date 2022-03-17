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
		Buffer<TileTerrain> terrain = new Buffer<TileTerrain>( 2, 2 );
		for( int r = 0; r < terrain.Size.Rows; r++) {
			for (int c = 0; c < terrain.Size.Columns; c++) {
				terrain[r][c] = TileTerrain.Grass;
			}
		}
		Mock<IMutableFileManager> fileManager = new Mock<IMutableFileManager>( MockBehavior.Strict );

		Image<Rgba32> image = new Image<Rgba32>( 122, 160 );
		_imageFactory
			.Setup( f => f.CreateImage( 122, 160 ) )
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
		Image<Rgba32> image = new Image<Rgba32>( 1, 1 );

		Id<FileMetadata> mountainId = new Id<FileMetadata>( "mountain" );
		Mock<Stream> stream = new Mock<Stream>( MockBehavior.Strict );
		imageFactory
			.Setup( f => f.LoadImageAsync( stream.Object, It.IsAny<CancellationToken>() ) )
			.Returns( Task.FromResult( image ) );
		resourceFileManager
			.Setup( rfm => rfm.MountainTileId )
			.Returns( mountainId );
		resourceFileManager
			.Setup( rfm => rfm.TryGetContentAsync( mountainId, It.IsAny<AsyncStreamHandler>(), It.IsAny<CancellationToken>() ) )
			.Callback<Id<FileMetadata>, AsyncStreamHandler, CancellationToken>( (fileId, func, token) => {
				func.Invoke( stream.Object ).Wait( token );
			} )
			.Returns( Task.FromResult( true ) );

		Id<FileMetadata> hillId = new Id<FileMetadata>( "hill" );
		resourceFileManager
			.Setup( rfm => rfm.HillTileId )
			.Returns( hillId );
		resourceFileManager
			.Setup( rfm => rfm.TryGetContentAsync( hillId, It.IsAny<AsyncStreamHandler>(), It.IsAny<CancellationToken>() ) )
			.Callback<Id<FileMetadata>, AsyncStreamHandler, CancellationToken>( ( fileId, func, token ) => {
				func.Invoke( stream.Object ).Wait( token );
			} )
			.Returns( Task.FromResult( true ) );

		Id<FileMetadata> forestId = new Id<FileMetadata>( "forest" );
		resourceFileManager
			.Setup( rfm => rfm.ForestTileId )
			.Returns( forestId );
		resourceFileManager
			.Setup( rfm => rfm.TryGetContentAsync( forestId, It.IsAny<AsyncStreamHandler>(), It.IsAny<CancellationToken>() ) )
			.Callback<Id<FileMetadata>, AsyncStreamHandler, CancellationToken>( ( fileId, func, token ) => {
				func.Invoke( stream.Object ).Wait( token );
			} )
			.Returns( Task.FromResult( true ) );

		Id<FileMetadata> grassId = new Id<FileMetadata>( "grass" );
		resourceFileManager
			.Setup( rfm => rfm.GrassTileId )
			.Returns( grassId );
		resourceFileManager
			.Setup( rfm => rfm.TryGetContentAsync( grassId, It.IsAny<AsyncStreamHandler>(), It.IsAny<CancellationToken>() ) )
			.Callback<Id<FileMetadata>, AsyncStreamHandler, CancellationToken>( ( fileId, func, token ) => {
				func.Invoke( stream.Object ).Wait( token );
			} )
			.Returns( Task.FromResult( true ) );

		Id<FileMetadata> coastId = new Id<FileMetadata>( "coast" );
		resourceFileManager
			.Setup( rfm => rfm.CoastTileId )
			.Returns( coastId );
		resourceFileManager
			.Setup( rfm => rfm.TryGetContentAsync( coastId, It.IsAny<AsyncStreamHandler>(), It.IsAny<CancellationToken>() ) )
			.Callback<Id<FileMetadata>, AsyncStreamHandler, CancellationToken>( ( fileId, func, token ) => {
				func.Invoke( stream.Object ).Wait( token );
			} )
			.Returns( Task.FromResult( true ) );

		Id<FileMetadata> oceanId = new Id<FileMetadata>( "ocean" );
		resourceFileManager
			.Setup( rfm => rfm.OceanTileId )
			.Returns( oceanId );
		resourceFileManager
			.Setup( rfm => rfm.TryGetContentAsync( oceanId, It.IsAny<AsyncStreamHandler>(), It.IsAny<CancellationToken>() ) )
			.Callback<Id<FileMetadata>, AsyncStreamHandler, CancellationToken>( ( fileId, func, token ) => {
				func.Invoke( stream.Object ).Wait( token );
			} )
			.Returns( Task.FromResult( true ) );
	}
}
