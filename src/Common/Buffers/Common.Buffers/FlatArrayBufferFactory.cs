namespace Common.Buffers;

[System.Diagnostics.CodeAnalysis.SuppressMessage( "Performance", "CA1812:An internal (assembly-level) type is never instantiated.", Justification = "This class is instantiated via DI." )]
internal sealed class FlatArrayBufferFactory : IBufferFactory {

	IBuffer<T> IBufferFactory.Create<T>( ISize size, T initialValue ) {
		IBuffer<T> buffer = new FlatArrayBuffer<T>( size, initialValue );
		for( int r = 0; r < size.Height; r++ ) {
			for( int c = 0; c < size.Width; c++ ) {
				buffer[c, r] = initialValue;
			}
		}

		return buffer;
	}

	IBuffer<T> IBufferFactory.Create<T>( ISize size ) {
		return new FlatArrayBuffer<T>( size );
	}

	IBuffer<T> IBufferFactory.Create<T>( int columns, int rows ) {
		return new FlatArrayBuffer<T>( new Point( columns, rows ) );
	}
}
