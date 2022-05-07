namespace Common.Buffer;

public interface IBuffer<T> {

	public Size Size { get; }

	[System.Diagnostics.CodeAnalysis.SuppressMessage( "Performance", "CA1819:Properties should not return arrays", Justification = "Abstracting away a multi-dimensional float array" )]
	public T[] this[int row] { get; }

	void Apply(
		Func<T, T> action
	);

	void CopyFrom(
		IBuffer<T> source
	);
}
