using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Common.Renderers.Bitmap;

internal sealed class ImageFactory : IImageFactory {
	Image<Argb32> IImageFactory.Create( int width, int height ) {
		return new Image<Argb32>( width, height );
	}

	async Task<Image<Argb32>?> IImageFactory.LoadAsync(
		Stream stream,
		CancellationToken cancellationToken
	) {
		return await Image.LoadAsync<Argb32>( stream, cancellationToken ).ConfigureAwait( false );
	}
}

