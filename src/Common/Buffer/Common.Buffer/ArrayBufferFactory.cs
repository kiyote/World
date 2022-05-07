namespace Common.Buffer;

internal sealed class ArrayBufferFactory : IBufferFactory {
	IBuffer<T> IBufferFactory.Create<T>( Size size ) {
		return new ArrayBuffer<T>( size );
	}

	IBuffer<T> IBufferFactory.Create<T>( int columns, int rows ) {
		return new ArrayBuffer<T>( new Size( columns, rows ) );
	}
}
