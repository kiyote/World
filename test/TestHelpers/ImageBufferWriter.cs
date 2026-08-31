using System.IO.Abstractions;
using Kiyote.Buffers;
using Kiyote.Imaging;
using Kiyote.Imaging.Png;

namespace TestHelpers;

public sealed class ImageBufferWriter : IBufferWriter<float>, IBufferWriter<bool> {

	private readonly IBufferFactory _bufferFactory;
	private readonly string _filename;

	public ImageBufferWriter(
		IBufferFactory bufferFactory,
		string filename
	) {
		_bufferFactory = bufferFactory;
		_filename = filename;
	}

	Task IBufferWriter<float>.WriteAsync(
		IBuffer<float> buffer
	) {
		IImageWriter image = new PngWriter( new FileSystem() );

		IBuffer<byte> output = _bufferFactory.Create<byte>(buffer.Columns, buffer.Rows, 0);

		for( int r = 0; r < buffer.Rows; r++ ) {
			for (int c = 0; c < buffer.Columns; c++ ) {
				float value = buffer[c, r];
				output[c, r] = (byte)(255 * value);
			}
		}

		image.WriteImage(_filename, output);
		return Task.CompletedTask;
	}

	Task IBufferWriter<bool>.WriteAsync(
		IBuffer<bool> buffer
	) {
		IImageWriter image = new PngWriter( new FileSystem() );

		image.WriteImage(_filename, buffer);
		return Task.CompletedTask;
	}
}
