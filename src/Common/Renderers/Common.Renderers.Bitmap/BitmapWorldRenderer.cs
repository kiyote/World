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

	public BitmapWorldRenderer(
		IResourceFileManager resourceFileManager
	) {
		_resourceFileManager = resourceFileManager;
	}

	async Task IWorldRenderer.RenderTerrainToAsync(
		IMutableFileManager fileManager,
		Id<FileMetadata> fileId,
		TileTerrain[,] terrain,
		CancellationToken cancellationToken
	) {
		using Stream mountainStream = await _resourceFileManager.GetContentAsync( _resourceFileManager.MountainTerrainId, cancellationToken ).ConfigureAwait( false );
		Image<Argb32> mountain = await Image.LoadAsync<Argb32>( mountainStream, cancellationToken ).ConfigureAwait( false );

		using Stream hillStream = await _resourceFileManager.GetContentAsync( _resourceFileManager.HillTerrainId, cancellationToken ).ConfigureAwait( false );
		Image<Argb32> hill = await Image.LoadAsync<Argb32>( hillStream, cancellationToken ).ConfigureAwait( false );

		using Stream plainsStream = await _resourceFileManager.GetContentAsync( _resourceFileManager.PlainsTerrainId, cancellationToken ).ConfigureAwait( false );
		Image<Argb32> plains = await Image.LoadAsync<Argb32>( plainsStream, cancellationToken ).ConfigureAwait( false );

		int rows = terrain.GetLength( 0 );
		int columns = terrain.GetLength( 1 );
		using Image<Argb32> img = new Image<Argb32>( (columns * 53) + 16, (rows * 64) + 32 );

		for (int r = 0; r < rows; r++) {
			for (int c = 0; c < columns; c++) {
				int x = 53 * c;
				int y = (64 * r) + ( 32 * ( x & 1 ) );

				switch (terrain[c, r]) {
					case TileTerrain.Mountain:
						img.Mutate( i => i.DrawImage( mountain, new Point(x, y), 1.0f ) );
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

