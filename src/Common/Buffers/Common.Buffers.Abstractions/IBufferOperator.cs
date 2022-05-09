namespace Common.Buffers;

public interface IBufferOperator {
	/// <summary>
	/// Performs the supplied <paramref name="op"/> on each value in the
	/// <paramref name="source"/> buffer, storing the result in the
	/// <paramref name="output"/> buffer.
	/// </summary>
	/// <typeparam name="T">The type of each value in the buffer.</typeparam>
	/// <param name="source">The buffer to be operated upon.</param>
	/// <param name="op">A function that takes the value from the buffer and returns the desired value.</param>
	/// <param name="output">The buffer to receive the resulting value.</param>
	void Perform<T>(
		IBuffer<T> source,
		Func<T, T> op,
		IBuffer<T> output
	);

	/// <summary>
	/// Performs the supplied <paramref name="op"/> on the correspond pair of 
	/// values from <paramref name="source1"/> and <paramref name="source2"/>,
	/// storing the result in the <paramref name="output"/> buffer.
	/// </summary>
	/// <typeparam name="T">The type of each value in the buffer.</typeparam>
	/// <param name="source1">Supplies the first value to the <paramref name="op"/> function.</param>
	/// <param name="source2">Supplies the second value to the <paramref name="op"/> function.</param>
	/// <param name="op">A function that takes a value from each source buffer and returns the desired value.</param>
	/// <param name="output">The buffer to receive the resulting value.</param>
	void Perform<T>(
		IBuffer<T> source1,
		IBuffer<T> source2,
		Func<T, T, T> op,
		IBuffer<T> output
	);
}
