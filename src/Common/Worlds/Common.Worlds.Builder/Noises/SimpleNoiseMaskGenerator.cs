namespace Common.Worlds.Builder.Noises;

internal sealed class SimpleNoiseMaskGenerator : INoiseMaskGenerator {

	private readonly INoiseOperator _noiseOperator;

	public SimpleNoiseMaskGenerator(
		INoiseOperator noiseOperator
	) {
		_noiseOperator = noiseOperator;
	}

	public float[,] Circle(
		Size size
	) {
		float cx = (float)size.Columns / 2.0f;
		float cy = (float)size.Rows / 2.0f;
		float[,] gradient = new float[size.Columns, size.Rows];
		float maxValue = float.MinValue;
		float minValue = float.MaxValue;
		for( int r = 0; r < size.Rows; r++ ) {
			for( int c = 0; c < size.Columns; c++ ) {
				double realDistance = Math.Pow( cx - c, 2 ) + Math.Pow( cy - r, 2 );
				float distance = (float)Math.Sqrt( realDistance );
				if( distance > maxValue ) {
					maxValue = distance;
				}
				if( distance < minValue ) {
					minValue = distance;
				}
				gradient[c, r] = distance;
			}
		}

		// Push the edge from the corner to the mid-point
		float target = gradient[size.Columns / 2, 0];
		gradient = _noiseOperator.Threshold( ref gradient, minValue, minValue, target, maxValue );
		gradient = _noiseOperator.Normalize( ref gradient );
		gradient = _noiseOperator.Invert( ref gradient );

		return gradient;
	}
}

