﻿namespace Common.Buffer.FloatingPoint;

internal class FloatBufferFilterOperators : IFloatBufferFilterOperators {

	private readonly IBufferFactory _bufferFactory;

	public FloatBufferFilterOperators(
		IBufferFactory bufferFactory
	) {
		_bufferFactory = bufferFactory;
	}

	IBufferFilterOperators<float> IBufferFilterOperators<float>.GateHigh(
		IBuffer<float> source,
		float threshold,
		IBuffer<float> output
	) {
		DoSingleOperator(
			source,
			( float value ) => {
				if( value < threshold ) {
					return 0.0f;
				}
				return 1.0f;
			},
			output
		);
		return this;
	}

	IBuffer<float> IBufferFilterOperators<float>.GateHigh(
		IBuffer<float> source,
		float threshold
	) {
		IBuffer<float> output = _bufferFactory.Create<float>( source.Size );
		( this as IFloatBufferFilterOperators ).GateHigh( source, threshold, output );
		return output;
	}

	IBufferFilterOperators<float> IBufferFilterOperators<float>.GateLow(
		IBuffer<float> source,
		float threshold,
		IBuffer<float> output
	) {
		DoSingleOperator(
			source,
			( float value ) => {
				if( value > threshold ) {
					return 1.0f;
				}
				return 0.0f;
			},
			output
		);
		return this;
	}

	IBuffer<float> IBufferFilterOperators<float>.GateLow(
		IBuffer<float> source,
		float threshold
	) {
		IBuffer<float> output = _bufferFactory.Create<float>( source.Size );
		( this as IFloatBufferFilterOperators ).GateLow( source, threshold, output );
		return output;
	}

	IBufferFilterOperators<float> IBufferFilterOperators<float>.Invert(
		IBuffer<float> source,
		IBuffer<float> output
	) {
		DoSingleOperator(
			source,
			( float value ) => {
				return 1.0f - value;
			},
			output
		);
		return this;
	}

	IBuffer<float> IBufferFilterOperators<float>.Invert(
		IBuffer<float> source
	) {
		IBuffer<float> output = _bufferFactory.Create<float>( source.Size );
		( this as IFloatBufferFilterOperators ).Invert( source, output );
		return output;
	}

	IBufferFilterOperators<float> IBufferFilterOperators<float>.Quantize(
		IBuffer<float> source,
		float[] ranges,
		IBuffer<float> output
	) {
		float level = 1.0f / ( ranges.Length + 1 );
		DoSingleOperator(
			source,
			( float value ) => {
				for( int i = 0; i < ranges.Length; i++ ) {
					if( value < ranges[i] ) {
						return level * i;
					}
				}

				throw new InvalidOperationException();
			},
			output
		);
		return this;
	}

	IBuffer<float> IBufferFilterOperators<float>.Quantize(
		IBuffer<float> source,
		float[] ranges
	) {
		IBuffer<float> output = _bufferFactory.Create<float>( source.Size );
		( this as IFloatBufferFilterOperators ).Quantize( source, ranges, output );
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
}
