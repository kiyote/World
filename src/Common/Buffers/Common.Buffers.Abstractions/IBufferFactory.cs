namespace Common.Buffers;

public interface IBufferFactory {

	IBuffer<T> Create<T>( Size size, T initialValue );

	IBuffer<T> Create<T>( Size size );

	IBuffer<T> Create<T>( int columns, int rows );
}
