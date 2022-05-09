namespace Common.Buffer.Unit;

[System.Diagnostics.CodeAnalysis.SuppressMessage( "Performance", "CA1812:An internal (assembly-level) type is never instantiated.", Justification = "This class is instantiated via DI." )]
internal sealed class UnitBufferArithmeticOperators : IUnitBufferArithmeticOperators {

	private readonly IBufferFactory _bufferFactory;

	public UnitBufferArithmeticOperators(
		IBufferFactory bufferFactory
	) {
		_bufferFactory = bufferFactory;
	}

	IBufferArithmeticOperators<float> IBufferArithmeticOperators<float>.Add(
		IBuffer<float> source,
		float amount,
		IBuffer<float> output
	) {
		DoSingleOperator(
			source,
			( float value ) => {
				return value + amount;
			},
			output
		);

		return this;
	}

	IBufferArithmeticOperators<float> IBufferArithmeticOperators<float>.Add(
		IBuffer<float> source,
		IBuffer<float> amount,
		IBuffer<float> output
	) {
		DoMultiOperator(
			source,
			amount,
			( float left, float right ) => {
				return left + right;
			},
			output
		);
		return this;
	}

	IBuffer<float> IBufferArithmeticOperators<float>.Add(
		IBuffer<float> source,
		float amount
	) {
		IBuffer<float> output = _bufferFactory.Create<float>( source.Size );
		( this as IUnitBufferArithmeticOperators ).Add( source, amount, output );
		return output;
	}

	IBuffer<float> IBufferArithmeticOperators<float>.Add(
		IBuffer<float> source,
		IBuffer<float> amount
	) {
		IBuffer<float> output = _bufferFactory.Create<float>( source.Size );
		( this as IUnitBufferArithmeticOperators ).Add( source, amount, output );
		return output;
	}

	IBufferArithmeticOperators<float> IBufferArithmeticOperators<float>.Multiply(
		IBuffer<float> source,
		float amount,
		IBuffer<float> output
	) {
		DoSingleOperator(
			source,
			( float value ) => {
				return value * amount;
			},
			output
		);

		return this;
	}

	IBufferArithmeticOperators<float> IBufferArithmeticOperators<float>.Multiply(
		IBuffer<float> source,
		IBuffer<float> amount,
		IBuffer<float> output
	) {
		DoMultiOperator(
			source,
			amount,
			( float left, float right ) => {
				return left * right;
			},
			output
		);
		return this;
	}

	IBuffer<float> IBufferArithmeticOperators<float>.Multiply(
		IBuffer<float> source,
		float amount
	) {
		IBuffer<float> output = _bufferFactory.Create<float>( source.Size );
		( this as IUnitBufferArithmeticOperators ).Multiply( source, amount, output );
		return output;
	}

	IBuffer<float> IBufferArithmeticOperators<float>.Multiply(
		IBuffer<float> source,
		IBuffer<float> amount
	) {
		IBuffer<float> output = _bufferFactory.Create<float>( source.Size );
		( this as IUnitBufferArithmeticOperators ).Multiply( source, amount, output );
		return output;
	}

	IBufferArithmeticOperators<float> IBufferArithmeticOperators<float>.Subtract(
		IBuffer<float> source,
		IBuffer<float> amount,
		IBuffer<float> output
	) {
		DoMultiOperator(
			source,
			amount,
			( float left, float right ) => {
				return left - right;
			},
			output
		);
		return this;
	}

	IBufferArithmeticOperators<float> IBufferArithmeticOperators<float>.Subtract(
		IBuffer<float> source,
		float amount,
		IBuffer<float> output
	) {
		DoSingleOperator(
			source,
			( float value ) => {
				return value - amount;
			},
			output
		);

		return this;
	}

	IBuffer<float> IBufferArithmeticOperators<float>.Subtract(
		IBuffer<float> source,
		IBuffer<float> amount
	) {
		IBuffer<float> output = _bufferFactory.Create<float>( source.Size );
		( this as IUnitBufferArithmeticOperators ).Subtract( source, amount, output );
		return output;
	}

	IBuffer<float> IBufferArithmeticOperators<float>.Subtract(
		IBuffer<float> source,
		float amount
	) {
		IBuffer<float> output = _bufferFactory.Create<float>( source.Size );
		( this as IUnitBufferArithmeticOperators ).Subtract( source, amount, output );
		return output;
	}

	private static void DoSingleOperator(
		IBuffer<float> a,
		Func<float, float> op,
		IBuffer<float> output
	) {
		int rows = a.Size.Rows;
		int columns = a.Size.Columns;

		for( int r = 0; r < rows; r++ ) {
			for( int c = 0; c < columns; c++ ) {
				output[r][c] = op( a[r][c] );
			}
		}
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
