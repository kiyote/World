using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Common.Renderers.Bitmap;

internal sealed class ImageFactory : IImageFactory {
	Image<Rgba32> IImageFactory.CreateImage( int width, int height ) {
		return new Image<Rgba32>( width, height );
	}

	Image<L8> IImageFactory.CreateMap( int width, int height ) {
		return new Image<L8>( width, height );
	}

	async Task<Image<Rgba32>?> IImageFactory.LoadImageAsync(
		Stream stream,
		CancellationToken cancellationToken
	) {
		return await Image.LoadAsync<Rgba32>( stream, cancellationToken ).ConfigureAwait( false );
	}
}

