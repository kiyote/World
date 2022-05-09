namespace Common.Buffers;

public interface IBufferFilterOperators<T> {

	IBufferFilterOperators<T> Invert(
		IBuffer<T> source,
		IBuffer<T> output
	);

	IBuffer<T> Invert(
		IBuffer<T> source
	);

	/// <summary>
	/// Sets <paramref name="output"/> to a high value if the 
	/// <paramref name="source"/> value is greater than the threshold.
	/// </summary>
	/// <remarks>
	/// Filters the <paramref name="source"/> buffer to one of two values,
	/// depending on the <paramref name="threshold"/> value.  If the value
	/// at the location in <paramref name="source"/> is greater than the
	/// <paramref name="threshold"/> the value is set high, otherwise the value
	/// is set low.
	/// </remarks>
	/// <param name="source">The buffer to read from.</param>
	/// <param name="threshold">The decision point for the value.</param>
	/// <param name="output">The buffer the result is written to.</param>
	/// <returns>
	/// Returns the set of filter operators for fluent expressions.
	/// </returns>
	IBufferFilterOperators<T> GateHigh(
		IBuffer<T> source,
		T threshold,
		IBuffer<T> output
	);

	/// <summary>
	/// Sets <paramref name="output"/> to a high value if the 
	/// <paramref name="source"/> value is greater than the threshold.
	/// </summary>
	/// <remarks>
	/// Filters the <paramref name="source"/> buffer to one of two values,
	/// depending on the <paramref name="threshold"/> value.  If the value
	/// at the location in <paramref name="source"/> is greater than the
	/// <paramref name="threshold"/> the value is set high, otherwise the value
	/// is set low.
	/// </remarks>
	/// <param name="source">The buffer to read from.</param>
	/// <param name="threshold">The decision point for the value.</param>
	/// <returns>
	/// Returns a newly created buffer containing the resulting values from the
	/// operation, equal in size to <paramref name="source"/>.
	/// </returns>
	IBuffer<T> GateHigh(
		IBuffer<T> source,
		T threshold
	);

	/// <summary>
	/// Sets <paramref name="output"/> to a low value if the 
	/// <paramref name="source"/> value is less than the threshold.
	/// </summary>
	/// <remarks>
	/// Filters the <paramref name="source"/> buffer to one of two values,
	/// depending on the <paramref name="threshold"/> value.  If the value
	/// at the location in <paramref name="source"/> is lower than the
	/// <paramref name="threshold"/> the value is set low, otherwise the value
	/// is set high.
	/// </remarks>
	/// <param name="source">The buffer to read from.</param>
	/// <param name="threshold">The decision point for the value.</param>
	/// <param name="output">The buffer the result is written to.</param>
	/// <returns>
	/// Returns the set of filter operators for fluent expressions.
	/// </returns>
	IBufferFilterOperators<T> GateLow(
		IBuffer<T> source,
		T threshold,
		IBuffer<T> output
	);

	/// <summary>
	/// Sets <paramref name="output"/> to a low value if the 
	/// <paramref name="source"/> value is less than the threshold.
	/// </summary>
	/// <remarks>
	/// Filters the <paramref name="source"/> buffer to one of two values,
	/// depending on the <paramref name="threshold"/> value.  If the value
	/// at the location in <paramref name="source"/> is lower than the
	/// <paramref name="threshold"/> the value is set low, otherwise the value
	/// is set high.
	/// </remarks>
	/// <param name="source">The buffer to read from.</param>
	/// <param name="threshold">The decision point for the value.</param>
	/// <returns>
	/// Returns a newly created buffer containing the resulting values from the
	/// operation, equal in size to <paramref name="source"/>.
	/// </returns>
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

