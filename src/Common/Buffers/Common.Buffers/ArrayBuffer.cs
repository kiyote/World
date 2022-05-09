namespace Common.Buffers;

internal sealed class ArrayBuffer<T>: IBuffer<T> {

	private readonly T[][] _buffer;
	private readonly Size _size;

	public ArrayBuffer(
		Size size
	) {
		if( size is null ) {
			throw new ArgumentNullException( nameof( size ) );
		}

		_size = size;
		_buffer = new T[size.Rows][];
		for( int r = 0; r < size.Rows; r++ ) {
			_buffer[r] = new T[size.Columns];
		}
	}

	public Size Size => _size;

	public T this[int column, int row] {
		get {
			return _buffer[row][column];
		}
		set {
			_buffer[row][column] = value;
		}
	}
}

