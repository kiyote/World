namespace Common.Core;

public sealed class Buffer<T> {

	private readonly T[][] _buffer;
	private readonly Size _size;

	public Buffer(
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

	public Buffer(
		int columns,
		int rows
	) : this( new Size( columns, rows ) ) {
	}

	public Size Size => _size;

	[System.Diagnostics.CodeAnalysis.SuppressMessage( "Performance", "CA1819:Properties should not return arrays", Justification = "Abstracting away a multi-dimensional float array" )]
	public T[] this[int row] {
		get {
			return _buffer[row];
		}
	}

	public void Apply(
		Func<T, T> action
	) {
		if (action is null) {
			throw new ArgumentNullException( nameof( action ) );
		}

		for (int r = 0; r < _size.Rows; r++) {
			for (int c = 0; c < _size.Columns; c++) {
				_buffer[r][c] = action( _buffer[r][c] );
			}
		}
	}

	public void CopyFrom(
		Buffer<T> source
	) {
		if( source is null ) {
			throw new ArgumentNullException( nameof( source ) );
		}

		for( int r = 0; r < _size.Rows; r++ ) {
			for( int c = 0; c < _size.Columns; c++ ) {
				_buffer[r][c] = source[r][c];
			}
		}
	}
}

