namespace Common.Worlds.Builder.Noises;

internal sealed class SimpleNoiseOperator : INoiseOperator {

	private readonly INeighbourLocator _neighbourLocator;

	public SimpleNoiseOperator(
		INeighbourLocator neighbourLocator
	) {
		_neighbourLocator = neighbourLocator;
	}

	INoiseOperator INoiseOperator.Threshold(
		Buffer<float> source,
		float minimum,
		float minimumValue,
		float maximum,
		float maximumValue,
		Buffer<float> output
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

	Buffer<float> INoiseOperator.Threshold(
		Buffer<float> source,
		float minimum,
		float minimumValue,
		float maximum,
		float maximumValue
	) {
		var output = new Buffer<float>( source.Size );
		( this as INoiseOperator ).Threshold( source, minimum, minimumValue, maximum, maximumValue, output );
		return output;
	}

	INoiseOperator INoiseOperator.Multiply(
		Buffer<float> source,
		float amount,
		bool clamp,
		Buffer<float> output
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

	Buffer<float> INoiseOperator.Multiply(
		Buffer<float> source,
		float amount,
		bool clamp
	) {
		var output = new Buffer<float>( source.Size );
		( this as INoiseOperator ).Multiply( source, amount, clamp, output );
		return output;
	}

	INoiseOperator INoiseOperator.Multiply(
		Buffer<float> source,
		Buffer<float> amount,
		bool clamp,
		Buffer<float> output
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
	Buffer<float> INoiseOperator.Multiply(
		Buffer<float> source,
		Buffer<float> amount,
		bool clamp
	) {
		var output = new Buffer<float>( source.Size );
		( this as INoiseOperator ).Multiply( source, amount, clamp, output );
		return output;
	}

	INoiseOperator INoiseOperator.Subtract(
		Buffer<float> source,
		float amount,
		bool clamp,
		Buffer<float> output
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

	Buffer<float> INoiseOperator.Subtract(
		Buffer<float> source,
		float amount,
		bool clamp
	) {
		Buffer<float> output = new Buffer<float>( source.Size );
		( this as INoiseOperator ).Subtract( source, amount, clamp, output );
		return output;
	}

	INoiseOperator INoiseOperator.Subtract(
		Buffer<float> source,
		Buffer<float> amount,
		bool clamp,
		Buffer<float> output
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

	Buffer<float> INoiseOperator.Subtract(
		Buffer<float> source,
		Buffer<float> amount,
		bool clamp
	) {
		var output = new Buffer<float>( source.Size );
		( this as INoiseOperator ).Subtract( source, amount, clamp, output );
		return output;
	}

	INoiseOperator INoiseOperator.Add(
		Buffer<float> source,
		float amount,
		bool clamp,
		Buffer<float> output
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

	Buffer<float> INoiseOperator.Add(
		Buffer<float> source,
		float amount,
		bool clamp
	) {
		Buffer<float> output = new Buffer<float>( source.Size );
		( this as INoiseOperator ).Add( source, amount, clamp, output );
		return output;
	}

	INoiseOperator INoiseOperator.Add(
		Buffer<float> source,
		Buffer<float> amount,
		bool clamp,
		Buffer<float> output
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
	Buffer<float> INoiseOperator.Add(
		Buffer<float> source,
		Buffer<float> amount,
		bool clamp
	) {
		var output = new Buffer<float>( source.Size );
		( this as INoiseOperator ).Add(source, amount, clamp, output );
		return output;
	}

	INoiseOperator INoiseOperator.And(
		Buffer<float> a,
		float thresholdA,
		Buffer<float> b,
		float thresholdB,
		Buffer<float> output
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

	Buffer<float> INoiseOperator.And(
		Buffer<float> a,
		float thresholdA,
		Buffer<float> b,
		float thresholdB
	) {
		var output = new Buffer<float>( a.Size );
		( this as INoiseOperator ).And( a, thresholdA, b, thresholdB, output );
		return output;
	}

	INoiseOperator INoiseOperator.Or(
		Buffer<float> a,
		float thresholdA,
		Buffer<float> b,
		float thresholdB,
		Buffer<float> output
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

	Buffer<float> INoiseOperator.Or(
		Buffer<float> a,
		float thresholdA,
		Buffer<float> b,
		float thresholdB
	) {
		var output = new Buffer<float>( a.Size );
		( this as INoiseOperator ).Or( a, thresholdA, b, thresholdB, output );
		return output;
	}

	INoiseOperator INoiseOperator.EdgeDetect(
		Buffer<float> source,
		float threshold,
		Buffer<float> output
	) {
		int rows = source.Size.Rows;
		int columns = source.Size.Columns;

		DoSingleRangeOperator(
			source,
			( int row, int column, Buffer<float> input, float value ) => {
				if( value == 0.0f ) {
					bool isEdge = false;
					IEnumerable<(int x, int y)> neighbours = _neighbourLocator.GetNeighbours( columns, rows, column, row );
					foreach( (int x, int y) in neighbours ) {
						if( input[y][x] > threshold ) {
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

	Buffer<float> INoiseOperator.EdgeDetect(
		Buffer<float> source,
		float threshold
	) {
		var output = new Buffer<float>( source.Size );
		( this as INoiseOperator ).EdgeDetect( source, threshold, output );
		return output;
	}

	INoiseOperator INoiseOperator.Invert(
		Buffer<float> source,
		Buffer<float> output
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

	Buffer<float> INoiseOperator.Invert(
		Buffer<float> source
	) {
		var output = new Buffer<float>( source.Size );
		( this as INoiseOperator ).Invert( source, output );
		return output;
	}

	INoiseOperator INoiseOperator.GateHigh(
		Buffer<float> source,
		float threshold,
		Buffer<float> output
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

	Buffer<float> INoiseOperator.GateHigh(
		Buffer<float> source,
		float threshold
	) {
		var output = new Buffer<float>( source.Size );
		( this as INoiseOperator ).GateHigh( source, threshold, output );
		return output;
	}

	INoiseOperator INoiseOperator.GateLow(
		Buffer<float> source,
		float threshold,
		Buffer<float> output
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

	Buffer<float> INoiseOperator.GateLow(
		Buffer<float> source,
		float threshold
	) {
		var output = new Buffer<float>( source.Size );
		( this as INoiseOperator ).GateLow(source, threshold, output );
		return output;
	}

	INoiseOperator INoiseOperator.Range(
		Buffer<float> source,
		float min,
		float max,
		Buffer<float> output
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

	Buffer<float> INoiseOperator.Range(
		Buffer<float> source,
		float min,
		float max
	) {
		var output = new Buffer<float>( source.Size );
		( this as INoiseOperator ).Range(source, min, max, output );
		return output;
	}

	INoiseOperator INoiseOperator.Normalize(
		Buffer<float> source,
		Buffer<float> output
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

	Buffer<float> INoiseOperator.Normalize(
		Buffer<float> source
	) {
		var output = new Buffer<float>( source.Size );
		( this as INoiseOperator ).Normalize(source, output);
		return output;
	}

	INoiseOperator INoiseOperator.Quantize(
		Buffer<float> source,
		float[] ranges,
		Buffer<float> output
	) {
		float level = 1.0f / (ranges.Length + 1);
		DoSingleOperator(
			source,
			( float value ) => {
				for( int i = 0; i < ranges.Length; i++ ) {
					if (value < ranges[i]) {
						return level * i;
					}
				}

				throw new InvalidOperationException();
			},
			output
		);
		return this;
	}

	Buffer<float> INoiseOperator.Quantize(
		Buffer<float> source,
		float[] ranges
	) {
		var output = new Buffer<float>( source.Size );
		( this as INoiseOperator ).Quantize( source, ranges, output );
		return output;
	}

	INoiseOperator INoiseOperator.Denoise(
		Buffer<float> source,
		Buffer<float> output
	) {
		int rows = source.Size.Rows;
		int columns = source.Size.Columns;

		DoSingleRangeOperator(
			source,
			( int row, int column, Buffer<float> input, float value ) => {
				IEnumerable<(int x, int y)> neighbours = _neighbourLocator.GetNeighbours( columns, rows, column, row );
				IEnumerable<float> values = neighbours.Select( neighbour => input[neighbour.y][neighbour.x] );
				values = values.Distinct();
				if (values.Count() == 1) {
					return values.First();
				}
				return value;
			},
			output
		);
		return this;
	}

	Buffer<float> INoiseOperator.Denoise(
		Buffer<float> source
	) {
		var output = new Buffer<float>( source.Size );
		( (INoiseOperator)this ).Denoise( source, output );
		return output;
	}


	INoiseOperator INoiseOperator.Mask(
		Buffer<float> source,
		Buffer<float> mask,
		float value,
		Buffer<float> output
	) {
		DoMultiOperator(
			source,
			mask,
			( sourceValue, maskValue ) => {
				if (maskValue == 1.0f) {
					return sourceValue;
				}
				return value;
			},
			output
		);
		return this;
	}

	Buffer<float> INoiseOperator.Mask(
		Buffer<float> source,
		Buffer<float> mask,
		float value
	) {
		var output = new Buffer<float>( source.Size );
		( this as INoiseOperator ).Mask( source, mask, value, output );
		return output;
	}

	private static void DoMultiOperator(
		Buffer<float> a,
		Buffer<float> b,
		Func<float, float, float> op,
		Buffer<float> output
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
		Buffer<float> a,
		Func<float, float> op,
		Buffer<float> output
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
		Buffer<float> a,
		Func<int, int, Buffer<float>, float, float> op,
		Buffer<float> output
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

