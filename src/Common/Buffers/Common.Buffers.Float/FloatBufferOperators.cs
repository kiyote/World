namespace Common.Buffers.Float;

[System.Diagnostics.CodeAnalysis.SuppressMessage( "Performance", "CA1812:An internal (assembly-level) type is never instantiated.", Justification = "This class is instantiated via DI." )]
internal sealed class FloatBufferOperators : IFloatBufferOperators {

	private readonly IBufferOperator _bufferOperator;

	public FloatBufferOperators(
		IBufferOperator bufferOperator
	) {
		_bufferOperator = bufferOperator;
	}

	void IFloatBufferOperators.Add(
		IBuffer<float> buffer,
		float value
	) {
		( this as IFloatBufferOperators ).Add( buffer, value, buffer );
	}

	void IFloatBufferOperators.Add(
		IBuffer<float> input,
		float amount,
		IBuffer<float> output
	) {
		_bufferOperator.Perform(
			input,
			( float value ) => {
				return value + amount;
			},
			output
		);
	}

	void IFloatBufferOperators.Add(
		IBuffer<float> buffer,
		IBuffer<float> amounts
	) {
		( this as IFloatBufferOperators ).Add( buffer, amounts, buffer );
	}

	void IFloatBufferOperators.Add(
		IBuffer<float> input,
		IBuffer<float> amounts,
		IBuffer<float> output
	) {
		_bufferOperator.Perform(
			input,
			amounts,
			( float value, float amount ) => {
				return value + amount;
			},
			output
		);
	}

	void IFloatBufferOperators.Multiply(
		IBuffer<float> buffer,
		float amount
	) {
		( this as IFloatBufferOperators ).Multiply( buffer, amount, buffer );
	}

	void IFloatBufferOperators.Multiply(
		IBuffer<float> input,
		float amount,
		IBuffer<float> output
	) {
		_bufferOperator.Perform(
			input,
			( float value ) => {
				return value * amount;
			},
			output
		);
	}

	void IFloatBufferOperators.Multiply(
		IBuffer<float> buffer,
		IBuffer<float> amounts
	) {
		( this as IFloatBufferOperators ).Multiply( buffer, amounts, buffer );
	}

	void IFloatBufferOperators.Multiply(
		IBuffer<float> input,
		IBuffer<float> amounts,
		IBuffer<float> output
	) {
		_bufferOperator.Perform(
			input,
			amounts,
			( float value, float amount ) => {
				return value * amount;
			},
			output
		);
	}

	void IFloatBufferOperators.Normalize(
		IBuffer<float> buffer,
		float minimum,
		float maximum
	) {
		( this as IFloatBufferOperators ).Normalize( buffer, minimum, maximum, buffer );
	}

	void IFloatBufferOperators.Normalize(
		IBuffer<float> input,
		float minimum,
		float maximum,
		IBuffer<float> output
	) {
		float desiredRange = maximum - minimum;
		float minValue = float.MaxValue;
		float maxValue = float.MinValue;
		int rows = input.Size.Rows;
		int columns = input.Size.Columns;
		for( int r = 0; r < rows; r++ ) {
			for( int c = 0; c < columns; c++ ) {
				float value = input[c, r];
				if( value < minValue ) {
					minValue = value;
				}
				if( value > maxValue ) {
					maxValue = value;
				}
			}
		}

		float actualRange = maxValue - minValue;
		float scale = 1.0f / actualRange;
		_bufferOperator.Perform( 
			input,
			( float value ) => {
				float result = value + Math.Abs( minValue );
				value *= scale; // Value will now be between 0..1

				return value * desiredRange;
			},
			output
		);

	}

	void IFloatBufferOperators.Subtract(
		IBuffer<float> buffer,
		float amount
	) {
		( this as IFloatBufferOperators ).Subtract( buffer, amount, buffer );
	}

	void IFloatBufferOperators.Subtract(
		IBuffer<float> input,
		float amount,
		IBuffer<float> output
	) {
		_bufferOperator.Perform(
			input,
			( float value ) => {
				return value - amount;
			},
			output
		);
	}

	void IFloatBufferOperators.Subtract(
		IBuffer<float> buffer,
		IBuffer<float> amounts
	) {
		( this as IFloatBufferOperators ).Subtract( buffer, amounts, buffer );
	}

	void IFloatBufferOperators.Subtract(
		IBuffer<float> input,
		IBuffer<float> amounts,
		IBuffer<float> output
	) {
		_bufferOperator.Perform(
			input,
			amounts,
			( float value, float amount ) => {
				return value - amount;
			},
			output
		);
	}
}
