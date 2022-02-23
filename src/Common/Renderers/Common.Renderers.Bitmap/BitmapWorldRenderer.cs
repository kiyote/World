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
		TileTerrain[,] terrain,
		TileTerrain terrainToRender,
		CancellationToken cancellationToken
	) {
		int rows = terrain.GetLength( 0 );
		int columns = terrain.GetLength( 1 );
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
		TileTerrain[,] terrain,
		CancellationToken cancellationToken
	) {
		Image<Rgba32>? mountain = null;
		bool loadedMountain = await _resourceFileManager.TryGetContentAsync(
				_resourceFileManager.MountainTerrainId,
				async ( Stream stream ) => {
					mountain = await _imageFactory.LoadImageAsync( stream, cancellationToken ).ConfigureAwait( false );
				},
				cancellationToken
			).ConfigureAwait( false );
		if( !loadedMountain ) {
			throw new InvalidOperationException();
		}

		Image<Rgba32>? hill = null;
		bool loadedHill = await _resourceFileManager.TryGetContentAsync(
				_resourceFileManager.HillTerrainId,
				async ( Stream stream ) => {
					hill = await _imageFactory.LoadImageAsync( stream, cancellationToken ).ConfigureAwait( false );
				},
				cancellationToken
			).ConfigureAwait( false );
		if( !loadedHill ) {
			throw new InvalidOperationException();
		}

		Image<Rgba32>? plains = null;
		bool loadedPlains = await _resourceFileManager.TryGetContentAsync(
				_resourceFileManager.PlainsTerrainId,
				async ( Stream stream ) => {
					plains = await _imageFactory.LoadImageAsync( stream, cancellationToken ).ConfigureAwait( false );
				},
				cancellationToken
			).ConfigureAwait( false );
		if( !loadedPlains ) {
			throw new InvalidOperationException();
		}

		int rows = terrain.GetLength( 0 );
		int columns = terrain.GetLength( 1 );
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
					case TileTerrain.Plain:
						img.Mutate( i => i.DrawImage( plains, new Point( x, y ), 1.0f ) );
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
}

