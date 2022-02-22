using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Common.Renderers.Bitmap;

internal interface IImageFactory {
	Task<Image<Argb32>?> LoadAsync( Stream stream, CancellationToken cancellationToken );

	Image<Argb32> Create( int width, int height );
}

