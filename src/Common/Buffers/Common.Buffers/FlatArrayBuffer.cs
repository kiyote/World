namespace Common.Buffers;

// This seems to be marginally more performant than ArrayBuffer
internal sealed class FlatArrayBuffer<T> : IBuffer<T> {

	private readonly T[] _buffer;
	private readonly Size _size;

	public FlatArrayBuffer(
		Size size
	) {
		if( size is null ) {
			throw new ArgumentNullException( nameof( size ) );
		}

		_size = size;
		_buffer = new T[size.Columns * size.Rows];
	}

	public FlatArrayBuffer(
		Size size,
		T initialValue
	) {
		if( size is null ) {
			throw new ArgumentNullException( nameof( size ) );
		}

		_size = size;
		_buffer = new T[size.Columns * size.Rows];
		Array.Fill( _buffer, initialValue );
	}

	public Size Size => _size;

	public T this[int column, int row] {
		get {
			int index = (row * _size.Columns) + column;
			return _buffer[index];
		}
		set {
			int index = ( row * _size.Columns ) + column;
			_buffer[index] = value;
		}
	}
}

