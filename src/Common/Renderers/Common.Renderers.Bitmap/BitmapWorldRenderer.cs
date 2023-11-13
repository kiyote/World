using Common.Buffers;
using Common.Files;
using Common.Files.Manager;
using Common.Files.Manager.Resource;
using Common.Worlds;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Common.Renderers.Bitmap;

[System.Diagnostics.CodeAnalysis.SuppressMessage( "Performance", "CA1812:An internal (assembly-level) type is never instantiated.", Justification = "This class is instantiated via DI." )]
internal sealed class BitmapWorldRenderer : IWorldRenderer {

	private readonly IResourceFileManager _resourceFileManager;
	private readonly IImageFactory _imageFactory;

	public BitmapWorldRenderer(
		IResourceFileManager resourceFileManager,
		IImageFactory imageFactory
	) {
		_resourceFileManager = resourceFileManager;
		_imageFactory = imageFactory;
	}

	async Task IWorldRenderer.RenderTerrainMapToAsync(
		IMutableFileManager fileManager,
		Id<FileMetadata> fileId,
		IBuffer<TileTerrain> terrain,
		TileTerrain terrainToRender,
		CancellationToken cancellationToken
	) {
		int rows = terrain.Size.Height;
		int columns = terrain.Size.Width;
		Image<L8> map = _imageFactory.CreateMap( columns, rows );

		for( int r = 0; r < rows; r++ ) {
			for( int c = 0; c < columns; c++ ) {

				if (terrain[c, r] == terrainToRender) {
					map[c, r] = new L8( 255 );
				} else {
					map[c, r] = new L8( 0 );
				}
			}
		}

		await fileManager.PutContentAsync(
			fileId,
			async ( Stream s ) => await map.SaveAsPngAsync( s, cancellationToken ).ConfigureAwait( false ),
			cancellationToken
		).ConfigureAwait( false );
	}

	async Task IWorldRenderer.RenderAtlasToAsync(
		IMutableFileManager fileManager,
		Id<FileMetadata> fileId,
		IBuffer<TileTerrain> terrain,
		CancellationToken cancellationToken
	) {
		Image<Rgba32> mountain = await LoadImageAsync( _resourceFileManager.MountainTileId, cancellationToken ).ConfigureAwait( false );
		Image<Rgba32> hill = await LoadImageAsync( _resourceFileManager.HillTileId, cancellationToken ).ConfigureAwait( false );
		Image<Rgba32> forest = await LoadImageAsync( _resourceFileManager.ForestTileId, cancellationToken ).ConfigureAwait( false );
		Image<Rgba32> grass = await LoadImageAsync( _resourceFileManager.GrassTileId, cancellationToken ).ConfigureAwait( false );
		Image<Rgba32> coast = await LoadImageAsync( _resourceFileManager.CoastTileId, cancellationToken ).ConfigureAwait( false );
		Image<Rgba32> ocean = await LoadImageAsync( _resourceFileManager.OceanTileId, cancellationToken ).ConfigureAwait( false );

		int rows = terrain.Size.Height;
		int columns = terrain.Size.Width;
		using Image<Rgba32> img = _imageFactory.CreateImage( ( columns * 53 ) + 16, ( rows * 64 ) + 32 );

		for( int r = 0; r < rows; r++ ) {
			for( int c = 0; c < columns; c++ ) {
				int x = 53 * c;
				int y = ( 64 * r ) + ( 32 * ( x & 1 ) );

				switch( terrain[c, r] ) {
					case TileTerrain.Mountain:
						img.Mutate( i => i.DrawImage( mountain, new Point( x, y ), 1.0f ) );
						break;
					case TileTerrain.Hill:
						img.Mutate( i => i.DrawImage( hill, new Point( x, y ), 1.0f ) );
						break;
						/*
					case TileTerrain.Forest:
						img.Mutate( i => i.DrawImage( forest, new Point( x, y ), 1.0f ) );
						break;
					case TileTerrain.Grass:
						img.Mutate( i => i.DrawImage( grass, new Point( x, y ), 1.0f ) );
						break;
					case TileTerrain.Coast:
						img.Mutate( i => i.DrawImage( coast, new Point( x, y ), 1.0f ) );
						break;
						*/
					case TileTerrain.Ocean:
						img.Mutate( i => i.DrawImage( ocean, new Point( x, y ), 1.0f ) );
						break;
					default:
						throw new InvalidOperationException();
				}
			}
		}

		await fileManager.PutContentAsync(
			fileId,
			async ( Stream s ) => await img.SaveAsPngAsync( s, cancellationToken ).ConfigureAwait( false ),
			cancellationToken
		).ConfigureAwait( false );
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage( "Maintainability", "CA1508:Avoid dead conditional code", Justification = "Code is not dead." )]
	private async Task<Image<Rgba32>> LoadImageAsync(
		Id<FileMetadata> fileId,
		CancellationToken cancellationToken
	) {
		Image<Rgba32>? image = null;
		bool loadedImage = await _resourceFileManager.TryGetContentAsync(
				fileId,
				async ( Stream stream ) => {
					image = await _imageFactory.LoadImageAsync( stream, cancellationToken ).ConfigureAwait( false );
				},
				cancellationToken
			).ConfigureAwait( false );
		if( image is null
			|| !loadedImage ) {
			throw new InvalidOperationException();
		}

		return image;
	}
}

