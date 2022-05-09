namespace Common.Buffer;

public interface IBufferArithmeticOperators<T> {

	IBufferArithmeticOperators<T> Subtract(
		IBuffer<T> source,
		IBuffer<T> amount,
		IBuffer<T> output
	);

	IBufferArithmeticOperators<T> Subtract(
		IBuffer<T> source,
		float amount,
		IBuffer<T> output
	);

	IBuffer<T> Subtract(
		IBuffer<T> source,
		IBuffer<T> amount
	);

	IBuffer<T> Subtract(
		IBuffer<T> source,
		T amount
	);

	IBufferArithmeticOperators<T> Add(
		IBuffer<T> source,
		float amount,
		IBuffer<T> output
	);

	IBufferArithmeticOperators<T> Add(
		IBuffer<T> source,
		IBuffer<T> amount,
		IBuffer<T> output
	);

	IBuffer<T> Add(
		IBuffer<T> source,
		T amount
	);

	IBuffer<T> Add(
		IBuffer<T> source,
		IBuffer<T> amount
	);

	IBufferArithmeticOperators<T> Multiply(
		IBuffer<T> source,
		T amount,
		IBuffer<T> output
	);

	IBufferArithmeticOperators<T> Multiply(
		IBuffer<T> source,
		IBuffer<T> amount,
		IBuffer<T> output
	);

	IBuffer<T> Multiply(
		IBuffer<T> source,
		T amount
	);

	IBuffer<T> Multiply(
		IBuffer<T> source,
		IBuffer<T> amount
	);
}
