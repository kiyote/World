using Kiyote.Buffers;

namespace TestHelpers;

public interface IBufferWriter<T> {

	Task WriteAsync(
		IBuffer<T> buffer
	);

}
