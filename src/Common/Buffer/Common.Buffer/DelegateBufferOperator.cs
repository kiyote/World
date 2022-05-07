namespace Common.Buffer;

[System.Diagnostics.CodeAnalysis.SuppressMessage( "Performance", "CA1812:An internal (assembly-level) type is never instantiated.", Justification = "This class is instantiated via DI." )]
internal sealed class DelegateBufferOperator : IBufferOperator {

	private readonly INeighbourLocator _neighbourLocator;

	public DelegateBufferOperator(
		INeighbourLocator neighbourLocator
	) {
		_neighbourLocator = neighbourLocator;
	}

	IBufferOperator IBufferOperator.Threshold(
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

	IBuffer<float> IBufferOperator.Threshold(
		IBuffer<float> source,
		float minimum,
		float minimumValue,
		float maximum,
		float maximumValue
	) {
		var output = new ArrayBuffer<float>( source.Size );
		( this as IBufferOperator ).Threshold( source, minimum, minimumValue, maximum, maximumValue, output );
		return output;
	}

	IBufferOperator IBufferOperator.Multiply(
		IBuffer<float> source,
		float amount,
		bool clamp,
		IBuffer<float> output
	) {
		DoSingleOperator(
			source,
			( float value ) => {
				if( clamp ) {
					return Math.Clamp( value * amount, 0.0f, 1.0f );
				}
				return value * amount;
			},
			output
		);

		return this;
	}

	IBuffer<float> IBufferOperator.Multiply(
		IBuffer<float> source,
		float amount,
		bool clamp
	) {
		var output = new ArrayBuffer<float>( source.Size );
		( this as IBufferOperator ).Multiply( source, amount, clamp, output );
		return output;
	}

	IBufferOperator IBufferOperator.Multiply(
		IBuffer<float> source,
		IBuffer<float> amount,
		bool clamp,
		IBuffer<float> output
	) {
		DoMultiOperator(
			source,
			amount,
			( float left, float right ) => {
				if( clamp ) {
					return Math.Clamp( left * right, 0.0f, 1.0f );
				}
				return left * right;
			},
			output
		);
		return this;
	}
	IBuffer<float> IBufferOperator.Multiply(
		IBuffer<float> source,
		IBuffer<float> amount,
		bool clamp
	) {
		var output = new ArrayBuffer<float>( source.Size );
		( this as IBufferOperator ).Multiply( source, amount, clamp, output );
		return output;
	}

	IBufferOperator IBufferOperator.Subtract(
		IBuffer<float> source,
		float amount,
		bool clamp,
		IBuffer<float> output
	) {
		DoSingleOperator(
			source,
			( float value ) => {
				if( clamp ) {
					return Math.Clamp( value - amount, 0.0f, 1.0f );
				}
				return value - amount;
			},
			output
		);

		return this;
	}

	IBuffer<float> IBufferOperator.Subtract(
		IBuffer<float> source,
		float amount,
		bool clamp
	) {
		var output = new ArrayBuffer<float>( source.Size );
		( this as IBufferOperator ).Subtract( source, amount, clamp, output );
		return output;
	}

	IBufferOperator IBufferOperator.Subtract(
		IBuffer<float> source,
		IBuffer<float> amount,
		bool clamp,
		IBuffer<float> output
	) {
		DoMultiOperator(
			source,
			amount,
			( float left, float right ) => {
				if( clamp ) {
					return Math.Clamp( left - right, 0.0f, 1.0f );
				}
				return left - right;
			},
			output
		);
		return this;
	}

	IBuffer<float> IBufferOperator.Subtract(
		IBuffer<float> source,
		IBuffer<float> amount,
		bool clamp
	) {
		var output = new ArrayBuffer<float>( source.Size );
		( this as IBufferOperator ).Subtract( source, amount, clamp, output );
		return output;
	}

	IBufferOperator IBufferOperator.Add(
		IBuffer<float> source,
		float amount,
		bool clamp,
		IBuffer<float> output
	) {
		DoSingleOperator(
			source,
			( float value ) => {
				if( clamp ) {
					return Math.Clamp( value + amount, 0.0f, 1.0f );
				}
				return value + amount;
			},
			output
		);

		return this;
	}

	IBuffer<float> IBufferOperator.Add(
		IBuffer<float> source,
		float amount,
		bool clamp
	) {
		var output = new ArrayBuffer<float>( source.Size );
		( this as IBufferOperator ).Add( source, amount, clamp, output );
		return output;
	}

	IBufferOperator IBufferOperator.Add(
		IBuffer<float> source,
		IBuffer<float> amount,
		bool clamp,
		IBuffer<float> output
	) {
		DoMultiOperator(
			source,
			amount,
			( float left, float right ) => {
				if( clamp ) {
					return Math.Clamp( left + right, 0.0f, 1.0f );
				}
				return left + right;
			},
			output
		);
		return this;
	}
	IBuffer<float> IBufferOperator.Add(
		IBuffer<float> source,
		IBuffer<float> amount,
		bool clamp
	) {
		var output = new ArrayBuffer<float>( source.Size );
		( this as IBufferOperator ).Add( source, amount, clamp, output );
		return output;
	}

	IBufferOperator IBufferOperator.And(
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

	IBuffer<float> IBufferOperator.And(
		IBuffer<float> a,
		float thresholdA,
		IBuffer<float> b,
		float thresholdB
	) {
		var output = new ArrayBuffer<float>( a.Size );
		( this as IBufferOperator ).And( a, thresholdA, b, thresholdB, output );
		return output;
	}

	IBufferOperator IBufferOperator.Or(
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

	IBuffer<float> IBufferOperator.Or(
		IBuffer<float> a,
		float thresholdA,
		IBuffer<float> b,
		float thresholdB
	) {
		var output = new ArrayBuffer<float>( a.Size );
		( this as IBufferOperator ).Or( a, thresholdA, b, thresholdB, output );
		return output;
	}

	IBufferOperator IBufferOperator.EdgeDetect(
		IBuffer<float> source,
		float threshold,
		IBuffer<float> output
	) {
		int rows = source.Size.Rows;
		int columns = source.Size.Columns;

		DoSingleRangeOperator(
			source,
			( int row, int column, IBuffer<float> input, float value ) => {
				if( value == 0.0f ) {
					bool isEdge = false;
					IEnumerable<Location> neighbours = _neighbourLocator.GetNeighbours( source.Size, column, row );
					foreach( Location location in neighbours ) {
						if( input[location.Row][location.Column] > threshold ) {
							isEdge = true;
						}
					}
					return isEdge ? 1.0f : 0.0f;
				}
				return 0.0f;
			},
			output
		);
		return this;
	}

	IBuffer<float> IBufferOperator.EdgeDetect(
		IBuffer<float> source,
		float threshold
	) {
		var output = new ArrayBuffer<float>( source.Size );
		( this as IBufferOperator ).EdgeDetect( source, threshold, output );
		return output;
	}

	IBufferOperator IBufferOperator.Invert(
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

	IBuffer<float> IBufferOperator.Invert(
		IBuffer<float> source
	) {
		var output = new ArrayBuffer<float>( source.Size );
		( this as IBufferOperator ).Invert( source, output );
		return output;
	}

	IBufferOperator IBufferOperator.GateHigh(
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

	IBuffer<float> IBufferOperator.GateHigh(
		IBuffer<float> source,
		float threshold
	) {
		var output = new ArrayBuffer<float>( source.Size );
		( this as IBufferOperator ).GateHigh( source, threshold, output );
		return output;
	}

	IBufferOperator IBufferOperator.GateLow(
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

	IBuffer<float> IBufferOperator.GateLow(
		IBuffer<float> source,
		float threshold
	) {
		var output = new ArrayBuffer<float>( source.Size );
		( this as IBufferOperator ).GateLow( source, threshold, output );
		return output;
	}

	IBufferOperator IBufferOperator.Range(
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

	IBuffer<float> IBufferOperator.Range(
		IBuffer<float> source,
		float min,
		float max
	) {
		var output = new ArrayBuffer<float>( source.Size );
		( this as IBufferOperator ).Range( source, min, max, output );
		return output;
	}

	IBufferOperator IBufferOperator.Normalize(
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

	IBuffer<float> IBufferOperator.Normalize(
		IBuffer<float> source
	) {
		var output = new ArrayBuffer<float>( source.Size );
		( this as IBufferOperator ).Normalize( source, output );
		return output;
	}

	IBufferOperator IBufferOperator.Quantize(
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

	IBuffer<float> IBufferOperator.Quantize(
		IBuffer<float> source,
		float[] ranges
	) {
		var output = new ArrayBuffer<float>( source.Size );
		( this as IBufferOperator ).Quantize( source, ranges, output );
		return output;
	}

	IBufferOperator IBufferOperator.Denoise(
		IBuffer<float> source,
		IBuffer<float> output
	) {
		int rows = source.Size.Rows;
		int columns = source.Size.Columns;

		DoSingleRangeOperator(
			source,
			( int row, int column, IBuffer<float> input, float value ) => {
				IEnumerable<Location> neighbours = _neighbourLocator.GetNeighbours( source.Size, column, row );
				IEnumerable<float> values = neighbours.Select( neighbour => input[neighbour.Row][neighbour.Column] );
				values = values.Distinct();
				if( values.Count() == 1 ) {
					return values.First();
				}
				return value;
			},
			output
		);
		return this;
	}

	IBuffer<float> IBufferOperator.Denoise(
		IBuffer<float> source
	) {
		var output = new ArrayBuffer<float>( source.Size );
		( (IBufferOperator)this ).Denoise( source, output );
		return output;
	}


	IBufferOperator IBufferOperator.Mask(
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

	IBuffer<float> IBufferOperator.Mask(
		IBuffer<float> source,
		IBuffer<float> mask,
		float value
	) {
		var output = new ArrayBuffer<float>( source.Size );
		( this as IBufferOperator ).Mask( source, mask, value, output );
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

	private static void DoSingleRangeOperator(
		IBuffer<float> a,
		Func<int, int, IBuffer<float>, float, float> op,
		IBuffer<float> output
	) {
		int rows = a.Size.Rows;
		int columns = a.Size.Columns;

		for( int r = 0; r < rows; r++ ) {
			for( int c = 0; c < columns; c++ ) {
				output[r][c] = op( c, r, a, a[r][c] );
			}
		}
	}
}
