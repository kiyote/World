using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Common.Renderers.Bitmap;

internal interface IImageFactory {
	Task<Image<Rgba32>?> LoadImageAsync( Stream stream, CancellationToken cancellationToken );

	Image<Rgba32> CreateImage( int width, int height );
	Image<L8> CreateMap( int width, int height );
}

