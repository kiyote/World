namespace Common.Buffers;

public interface IBuffer<T> {

	public Size Size { get; }

	public T this[int column, int row] { get; set; }
}
