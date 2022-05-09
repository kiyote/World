namespace Common.Buffers;

[System.Diagnostics.CodeAnalysis.SuppressMessage( "Performance", "CA1812:An internal (assembly-level) type is never instantiated.", Justification = "This class is instantiated via DI." )]
internal sealed class ArrayBufferFactory : IBufferFactory {
	IBuffer<T> IBufferFactory.Create<T>( Size size ) {
		return new ArrayBuffer<T>( size );
	}

	IBuffer<T> IBufferFactory.Create<T>( int columns, int rows ) {
		return new ArrayBuffer<T>( new Size( columns, rows ) );
	}
}
