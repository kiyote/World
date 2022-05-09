namespace Common.Buffer;

public interface IBufferFactory {

	IBuffer<T> Create<T>( Size size );

	IBuffer<T> Create<T>( int columns, int rows );
}
