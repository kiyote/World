using Kiyote.Geometry;

namespace Common.Buffers;

public interface IBuffer<T> {

	public ISize Size { get; }

	public T this[int column, int row] { get; set; }
}
