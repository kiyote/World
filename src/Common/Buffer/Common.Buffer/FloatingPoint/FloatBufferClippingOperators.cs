namespace Common.Buffer.FloatingPoint;

internal sealed class FloatBufferClippingOperators : IFloatBufferClippingOperators {

	IBufferClippingOperators<float> IBufferClippingOperators<float>.Mask(
		IBuffer<float> source,
		IBuffer<float> mask,
		float value,
		IBuffer<float> output
	) {
		DoMultiOperator(
			source,
			mask,
			( sourceValue, maskValue ) => {
				if( maskValue == 1.0f ) {
					return sourceValue;
				}
				return value;
			},
			output
		);
		return this;
	}

	IBuffer<float> IBufferClippingOperators<float>.Mask(
		IBuffer<float> source,
		IBuffer<float> mask,
		float value
	) {
		var output = new ArrayBuffer<float>( source.Size );
		( this as IFloatBufferClippingOperators ).Mask( source, mask, value, output );
		return output;
	}

	IBufferClippingOperators<float> IBufferClippingOperators<float>.Normalize(
		IBuffer<float> source,
		IBuffer<float> output
	) {
		float minValue = float.MaxValue;
		float maxValue = float.MinValue;
		int rows = source.Size.Rows;
		int columns = source.Size.Columns;
		for( int r = 0; r < rows; r++ ) {
			for( int c = 0; c < columns; c++ ) {
				float value = source[r][c];
				if( value < minValue ) {
					minValue = value;
				}
				if( value > maxValue ) {
					maxValue = value;
				}
			}
		}

		float range = maxValue - minValue;
		float scale = 1.0f / range;
		DoSingleOperator(
			source,
			( float value ) => {
				float result = value + Math.Abs( minValue );
				value *= scale;

				return value;
			},
			output
		);
		return this;
	}

	IBuffer<float> IBufferClippingOperators<float>.Normalize(
		IBuffer<float> source
	) {
		var output = new ArrayBuffer<float>( source.Size );
		( this as IFloatBufferClippingOperators ).Normalize( source, output );
		return output;
	}

	IBufferClippingOperators<float> IBufferClippingOperators<float>.Range(
		IBuffer<float> source,
		float min,
		float max,
		IBuffer<float> output
	) {
		DoSingleOperator(
			source,
			( float value ) => {
				if( ( value > min ) && ( value <= max ) ) {
					return 1.0f;
				} else {
					return 0.0f;
				}
			},
			output
		);
		return this;
	}

	IBuffer<float> IBufferClippingOperators<float>.Range(
		IBuffer<float> source,
		float min,
		float max
	) {
		var output = new ArrayBuffer<float>( source.Size );
		( this as IFloatBufferClippingOperators ).Range( source, min, max, output );
		return output;
	}

	IBufferClippingOperators<float> IBufferClippingOperators<float>.Threshold(
		IBuffer<float> source,
		float minimum,
		float minimumValue,
		float maximum,
		float maximumValue,
		IBuffer<float> output
	) {
		DoSingleOperator(
			source,
			( float value ) => {
				if( value < minimum ) {
					return minimumValue;
				} else if( value > maximum ) {
					return maximumValue;
				}
				return value;
			},
			output
		);

		return this;
	}

	IBuffer<float> IBufferClippingOperators<float>.Threshold(
		IBuffer<float> source,
		float minimum,
		float minimumValue,
		float maximum,
		float maximumValue
	) {
		var output = new ArrayBuffer<float>( source.Size );
		( this as IFloatBufferClippingOperators ).Threshold( source, minimum, minimumValue, maximum, maximumValue, output );
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
}
