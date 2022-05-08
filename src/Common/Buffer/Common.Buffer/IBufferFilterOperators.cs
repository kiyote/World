namespace Common.Buffer;

public interface IBufferFilterOperators<T> {

	IBufferFilterOperators<T> Invert(
		IBuffer<T> source,
		IBuffer<T> output
	);

	IBuffer<T> Invert(
		IBuffer<T> source
	);

	IBufferFilterOperators<T> GateHigh(
		IBuffer<T> source,
		T threshold,
		IBuffer<T> output
	);

	IBuffer<T> GateHigh(
		IBuffer<T> source,
		T threshold
	);

	IBufferFilterOperators<T> GateLow(
		IBuffer<T> source,
		T threshold,
		IBuffer<T> output
	);

	IBuffer<T> GateLow(
		IBuffer<T> source,
		T threshold
	);

	IBufferFilterOperators<T> Quantize(
		IBuffer<T> source,
		T[] ranges,
		IBuffer<T> output
	);

	IBuffer<T> Quantize(
		IBuffer<T> source,
		T[] ranges
	);
}

