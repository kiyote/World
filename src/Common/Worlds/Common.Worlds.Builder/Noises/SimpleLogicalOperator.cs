namespace Common.Worlds.Builder.Noises;

internal sealed class SimpleLogicalOperator : ILogicalOperator {
	float[,] ILogicalOperator.PerformAnd(
		ref float[,] a,
		float thresholdA,
		ref float[,] b,
		float thresholdB
	) {
		return DoOperator( ref a, ref b, ( float left, float right ) => {
			bool leftTrue = left > thresholdA;
			bool rightTrue = right > thresholdB;
			return leftTrue && rightTrue ? 1.0f : 0.0f;
		} );
	}

	float[,] ILogicalOperator.PerformOr(
		ref float[,] a,
		float thresholdA,
		ref float[,] b,
		float thresholdB
	) {
		return DoOperator( ref a, ref b, ( float left, float right ) => {
			bool leftTrue = left > thresholdA;
			bool rightTrue = right > thresholdB;
			return leftTrue || rightTrue ? 1.0f : 0.0f;
		} );
	}

	private static float[,] DoOperator(
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
}

