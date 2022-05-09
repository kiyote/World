namespace Common.Buffers;

public interface IBufferClippingOperators<T> {

	IBufferClippingOperators<T> Mask(
		IBuffer<T> source,
		IBuffer<T> mask,
		T value,
		IBuffer<T> output
	);

	IBuffer<T> Mask(
		IBuffer<T> source,
		IBuffer<T> mask,
		T value
	);


	IBufferClippingOperators<T> Range(
		IBuffer<T> source,
		T min,
		T max,
		IBuffer<T> output
	);

	IBuffer<T> Range(
		IBuffer<T> source,
		T min,
		T max
	);

	IBufferClippingOperators<T> Normalize(
		IBuffer<T> source,
		IBuffer<T> output
	);

	IBuffer<T> Normalize(
		IBuffer<T> source
	);

	IBufferClippingOperators<T> Threshold(
		IBuffer<T> source,
		T minimum,
		T minimumValue,
		T maximum,
		T maximumValue,
		IBuffer<T> output
	);

	IBuffer<T> Threshold(
		IBuffer<T> source,
		T minimum,
		T minimumValue,
		T maximum,
		T maximumValue
	);
}
