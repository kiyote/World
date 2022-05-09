namespace Common.Buffers;

[System.Diagnostics.CodeAnalysis.SuppressMessage( "Naming", "CA1716:Identifiers should not match keywords", Justification = "These are simple logical operators and these are the best names." )]
public interface IBufferLogicalOperators<T> {

	IBufferLogicalOperators<T> And(
		IBuffer<T> a,
		T thresholdA,
		IBuffer<T> b,
		T thresholdB,
		IBuffer<T> output
	);

	IBuffer<T> And(
		IBuffer<T> a,
		T thresholdA,
		IBuffer<T> b,
		T thresholdB
	);

	IBufferLogicalOperators<T> Or(
		IBuffer<T> a,
		T thresholdA,
		IBuffer<T> b,
		T thresholdB,
		IBuffer<T> output
	);

	IBuffer<T> Or(
		IBuffer<T> a,
		T thresholdA,
		IBuffer<T> b,
		T thresholdB
	);
}
