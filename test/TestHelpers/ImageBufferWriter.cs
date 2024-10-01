using Kiyote.Buffers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace TestHelpers;

public sealed class ImageBufferWriter : IBufferWriter<float>, IBufferWriter<bool> {

	private readonly string _filename;

	public ImageBufferWriter(
		string filename
	) {
		_filename = filename;
	}

	async Task IBufferWriter<float>.WriteAsync(
		IBuffer<float> buffer
	) {
		using Image<L8> image = new Image<L8>( buffer.Size.Width, buffer.Size.Height );

		for( int r = 0; r < buffer.Size.Height; r++ ) {
			for (int c = 0; c < buffer.Size.Width; c++ ) {
				float value = buffer[c, r];
				image[c, r] = new L8((byte)(255 * value));
			}
		}

		await image.SaveAsPngAsync( _filename ).ConfigureAwait( false );
	}

	async Task IBufferWriter<bool>.WriteAsync(
		IBuffer<bool> buffer
	) {
		using Image<L8> image = new Image<L8>( buffer.Size.Width, buffer.Size.Height );

		L8 white = new L8( 255 );
		L8 black = new L8( 0 );
		for( int r = 0; r < buffer.Size.Height; r++ ) {
			for( int c = 0; c < buffer.Size.Width; c++ ) {
				image[c, r] = buffer[c, r] ? white : black;
			}
		}

		await image.SaveAsPngAsync( _filename ).ConfigureAwait( false );
	}
}
