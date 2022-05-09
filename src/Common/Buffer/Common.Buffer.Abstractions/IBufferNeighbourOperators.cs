namespace Common.Buffer;

public interface IBufferNeighbourOperators<T> {

	IBufferNeighbourOperators<T> EdgeDetect(
		IBuffer<T> source,
		T threshold,
		IBuffer<T> output
	);

	IBuffer<T> EdgeDetect(
		IBuffer<T> source,
		T threshold
	);

	IBufferNeighbourOperators<T> Denoise(
		IBuffer<T> source,
		IBuffer<T> output
	);

	IBuffer<T> Denoise(
		IBuffer<T> source
	);
}

