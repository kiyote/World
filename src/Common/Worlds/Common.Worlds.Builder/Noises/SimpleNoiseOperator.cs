namespace Common.Worlds.Builder.Noises;

internal sealed class SimpleNoiseOperator : INoiseOperator {

	private readonly INeighbourLocator _neighbourLocator;

	public SimpleNoiseOperator(
		INeighbourLocator neighbourLocator
	) {
		_neighbourLocator = neighbourLocator;
	}

	float[,] INoiseOperator.Threshold(
		ref float[,] source,
		float minimum,
		float minimumValue,
		float maximum,
		float maximumValue
	) {
		return DoSingleOperator( ref source, ( float value ) => {
			if( value < minimum ) {
				return minimumValue;
			} else if( value > maximum ) {
				return maximumValue;
			}
			return value;
		} );
	}

	float[,] INoiseOperator.Multiply(
		ref float[,] source,
		float amount,
		bool clamp
	) {
		return DoSingleOperator( ref source, ( float value ) => {
			if( clamp ) {
				return Math.Clamp( value * amount, 0.0f, 1.0f );
			}
			return value * amount;
		} );
	}

	float[,] INoiseOperator.Subtract(
		ref float[,] source,
		float amount,
		bool clamp
	) {
		return DoSingleOperator( ref source, ( float value ) => {
			if( clamp ) {
				return Math.Clamp( value - amount, 0.0f, 1.0f );
			}
			return value - amount;
		} );
	}

	float[,] INoiseOperator.Add(
		ref float[,] source,
		float amount,
		bool clamp
	) {
		return DoSingleOperator( ref source, ( float value ) => {
			if( clamp ) {
				return Math.Clamp( value + amount, 0.0f, 1.0f );
			}
			return value + amount;
		} );
	}

	float[,] INoiseOperator.Multiply(
		ref float[,] source,
		ref float[,] amount,
		bool clamp
	) {
		return DoMultiOperator( ref source, ref amount, ( float left, float right ) => {
			if( clamp ) {
				return Math.Clamp( left * right, 0.0f, 1.0f );
			}
			return left * right;
		} );
	}

	float[,] INoiseOperator.And(
		ref float[,] a,
		float thresholdA,
		ref float[,] b,
		float thresholdB
	) {
		return DoMultiOperator( ref a, ref b, ( float left, float right ) => {
			bool leftTrue = left > thresholdA;
			bool rightTrue = right > thresholdB;
			return leftTrue && rightTrue ? 1.0f : 0.0f;
		} );
	}

	float[,] INoiseOperator.Or(
		ref float[,] a,
		float thresholdA,
		ref float[,] b,
		float thresholdB
	) {
		return DoMultiOperator( ref a, ref b, ( float left, float right ) => {
			bool leftTrue = left > thresholdA;
			bool rightTrue = right > thresholdB;
			return leftTrue || rightTrue ? 1.0f : 0.0f;
		} );
	}

	float[,] INoiseOperator.Subtract(
		ref float[,] source,
		ref float[,] amount,
		bool clamp
	) {
		return DoMultiOperator( ref source, ref amount, ( float left, float right ) => {
			if( clamp ) {
				return Math.Clamp( left - right, 0.0f, 1.0f );
			}
			return left - right;
		} );
	}

	float[,] INoiseOperator.Add(
		ref float[,] source,
		ref float[,] amount,
		bool clamp
	) {
		return DoMultiOperator( ref source, ref amount, ( float left, float right ) => {
			if( clamp ) {
				return Math.Clamp( left + right, 0.0f, 1.0f );
			}
			return left + right;
		} );
	}

	float[,] INoiseOperator.EdgeDetect(
		ref float[,] source,
		float threshold
	) {
		int rows = source.GetLength( 0 );
		int columns = source.GetLength( 1 );

		return DoSingleRangeOperator( ref source, ( int row, int column, float[,] input, float value ) => {
			if( value == 0.0f ) {
				bool isEdge = false;
				IEnumerable<(int x, int y)> neighbours = _neighbourLocator.GetNeighbours( columns, rows, column, row );
				foreach( (int x, int y) in neighbours ) {
					if( input[x, y] > threshold ) {
						isEdge = true;
					}
				}
				return isEdge ? 1.0f : 0.0f;
			}
			return 0.0f;
		} );
	}

	float[,] INoiseOperator.Invert(
		ref float[,] source
	) {
		return DoSingleOperator( ref source, ( float value ) => {
			return 1.0f - value;
		} );
	}

	float[,] INoiseOperator.GateHigh(
		ref float[,] source,
		float threshold
	) {
		return DoSingleOperator( ref source, ( float value ) => {
			if( value < threshold ) {
				return 0.0f;
			}
			return 1.0f;
		} );
	}

	float[,] INoiseOperator.GateLow(
		ref float[,] source,
		float threshold
	) {
		return DoSingleOperator( ref source, ( float value ) => {
			if( value > threshold ) {
				return 1.0f;
			}
			return 0.0f;
		} );
	}

	float[,] INoiseOperator.Range(
		ref float[,] source,
		float min,
		float max
	) {
		return DoSingleOperator( ref source, ( float value ) => {
			if( ( value > min ) && ( value <= max ) ) {
				return 1.0f;
			} else {
				return 0.0f;
			}
		} );
	}

	float[,] INoiseOperator.Normalize(
		ref float[,] source
	) {
		float minValue = float.MaxValue;
		float maxValue = float.MinValue;
		int rows = source.GetLength( 0 );
		int columns = source.GetLength( 1 );
		for( int r = 0; r < rows; r++ ) {
			for( int c = 0; c < columns; c++ ) {
				float value = source[c, r];
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
		return DoSingleOperator( ref source, ( float value ) => {
			float result = value + Math.Abs( minValue );
			value *= scale;

			return value;
		} );
	}

	private static float[,] DoMultiOperator(
		ref float[,] a,
		ref float[,] b,
		Func<float, float, float> op
	) {
		int rows = a.GetLength( 0 );
		int columns = a.GetLength( 1 );
		if( rows != b.GetLength( 0 )
			|| columns != b.GetLength( 1 )
		) {
			throw new InvalidOperationException( "Operands must be same dimensions." );
		}
		float[,] result = new float[rows, columns];

		for( int r = 0; r < rows; r++ ) {
			for( int c = 0; c < columns; c++ ) {
				result[c, r] = op( a[c, r], b[c, r] );
			}
		}

		return result;
	}

	private static float[,] DoSingleOperator(
		ref float[,] a,
		Func<float, float> op
	) {
		int rows = a.GetLength( 0 );
		int columns = a.GetLength( 1 );
		float[,] result = new float[rows, columns];

		for( int r = 0; r < rows; r++ ) {
			for( int c = 0; c < columns; c++ ) {
				result[c, r] = op( a[c, r] );
			}
		}

		return result;
	}

	private static float[,] DoSingleRangeOperator(
		ref float[,] a,
		Func<int, int, float[,], float, float> op
	) {
		int rows = a.GetLength( 0 );
		int columns = a.GetLength( 1 );
		float[,] result = new float[rows, columns];

		for( int r = 0; r < rows; r++ ) {
			for( int c = 0; c < columns; c++ ) {
				result[c, r] = op( c, r, a, a[c, r] );
			}
		}

		return result;
	}
}

