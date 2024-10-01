using Common.Files;
using Common.Files.Manager;
using Common.Worlds;
using Kiyote.Buffers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Common.Renderers.Minimap;

internal sealed class MinimapWorldRenderer : IWorldRenderer {

	private readonly IImageFactory _imageFactory;

	public MinimapWorldRenderer(
		IImageFactory imageFactory
	) {
		_imageFactory = imageFactory;
	}

	async Task IWorldRenderer.RenderAtlasToAsync(
		IMutableFileManager fileManager,
		Id<FileMetadata> fileId,
		IBuffer<TileTerrain> terrain,
		CancellationToken cancellationToken
	) {
		int rows = terrain.Size.Height;
		int columns = terrain.Size.Width;
		using Image<Rgba32> img = _imageFactory.CreateImage( ( columns * 2 ), ( rows * 2 ) + 1 );

		var ocean = new Rgba32( 28, 134, 238, 255 );
		var coast = new Rgba32( 110, 186, 231, 255 );
		var grass = new Rgba32( 175, 188, 158, 255 );
		var forest = new Rgba32( 183, 193, 140, 255 );
		var hill = new Rgba32( 220, 221, 190, 255 );
		var mountain = new Rgba32( 247, 247, 247, 255 );
		var unknown = new Rgba32( 0, 0, 0, 255 );

		for( int r = 0; r < rows; r++ ) {
			for( int c = 0; c < columns; c++ ) {

				int x = 2 * c;
				int y = ( 2 * r ) + ( 1 * ( c & 1 ) );

				switch( terrain[c, r] ) {
					case TileTerrain.Mountain:
						img[x + 0, y + 0] = mountain;
						img[x + 1, y + 0] = mountain;
						img[x + 0, y + 1] = mountain;
						img[x + 1, y + 1] = mountain;
						break;
					case TileTerrain.Hill:
						img[x + 0, y + 0] = hill;
						img[x + 1, y + 0] = hill;
						img[x + 0, y + 1] = hill;
						img[x + 1, y + 1] = hill;
						break;
						/*
					case TileTerrain.Forest:
						img[x + 0, y + 0] = forest;
						img[x + 1, y + 0] = forest;
						img[x + 0, y + 1] = forest;
						img[x + 1, y + 1] = forest;
						break;
					case TileTerrain.Grass:
						img[x + 0, y + 0] = grass;
						img[x + 1, y + 0] = grass;
						img[x + 0, y + 1] = grass;
						img[x + 1, y + 1] = grass;
						break;
					case TileTerrain.Coast:
						img[x + 0, y + 0] = coast;
						img[x + 1, y + 0] = coast;
						img[x + 0, y + 1] = coast;
						img[x + 1, y + 1] = coast;
						break;
						*/
					case TileTerrain.Ocean:
						img[x + 0, y + 0] = ocean;
						img[x + 1, y + 0] = ocean;
						img[x + 0, y + 1] = ocean;
						img[x + 1, y + 1] = ocean;
						break;
					default:
						/*
						img[x + 0, y + 0] = unknown;
						img[x + 1, y + 0] = unknown;
						img[x + 0, y + 1] = unknown;
						img[x + 1, y + 1] = unknown;
						break;
						*/
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

				if( terrain[c, r] == terrainToRender ) {
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
}
