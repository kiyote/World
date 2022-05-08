namespace Common.Buffer.FloatingPoint;

internal sealed class FloatBufferLogicalOperators: IFloatBufferLogicalOperators {

	private readonly IBufferFactory _bufferFactory;

	public FloatBufferLogicalOperators(
		IBufferFactory bufferFactory
	) {
		_bufferFactory = bufferFactory;
	}

	IBufferLogicalOperators<float> IBufferLogicalOperators<float>.And(
		IBuffer<float> a,
		float thresholdA,
		IBuffer<float> b,
		float thresholdB,
		IBuffer<float> output
	) {
		DoMultiOperator(
			a,
			b,
			( float left, float right ) => {
				bool leftTrue = left > thresholdA;
				bool rightTrue = right > thresholdB;
				return leftTrue && rightTrue ? 1.0f : 0.0f;
			},
			output
		);
		return this;
	}

	IBuffer<float> IBufferLogicalOperators<float>.And(
		IBuffer<float> a,
		float thresholdA,
		IBuffer<float> b,
		float thresholdB
	) {
		IBuffer<float> output = _bufferFactory.Create<float>( a.Size );
		( this as IFloatBufferLogicalOperators ).And( a, thresholdA, b, thresholdB, output );
		return output;
	}

	IBufferLogicalOperators<float> IBufferLogicalOperators<float>.Or(
		IBuffer<float> a,
		float thresholdA,
		IBuffer<float> b,
		float thresholdB,
		IBuffer<float> output
	) {
		DoMultiOperator(
			a,
			b,
			( float left, float right ) => {
				bool leftTrue = left > thresholdA;
				bool rightTrue = right > thresholdB;
				return leftTrue || rightTrue ? 1.0f : 0.0f;
			},
			output
		);
		return this;
	}

	IBuffer<float> IBufferLogicalOperators<float>.Or(
		IBuffer<float> a,
		float thresholdA,
		IBuffer<float> b,
		float thresholdB
	) {
		IBuffer<float> output = _bufferFactory.Create<float>( a.Size );
		( this as IFloatBufferLogicalOperators ).Or( a, thresholdA, b, thresholdB, output );
		return output;
	}

	private static void DoMultiOperator(
		IBuffer<float> a,
		IBuffer<float> b,
		Func<float, float, float> op,
		IBuffer<float> output
	) {
		int rows = a.Size.Rows;
		int columns = a.Size.Columns;
		if( rows != b.Size.Rows
			|| columns != b.Size.Columns
		) {
			throw new InvalidOperationException( "Operands must be same dimensions." );
		}
		for( int r = 0; r < rows; r++ ) {
			for( int c = 0; c < columns; c++ ) {
				output[r][c] = op( a[r][c], b[r][c] );
			}
		}
	}
}
