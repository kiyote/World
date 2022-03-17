namespace Common.Worlds.Builder.Noises;

internal sealed class SimpleNoiseMaskGenerator : INoiseMaskGenerator {

	private readonly INoiseOperator _noiseOperator;

	public SimpleNoiseMaskGenerator(
		INoiseOperator noiseOperator
	) {
		_noiseOperator = noiseOperator;
	}

	INoiseMaskGenerator INoiseMaskGenerator.Circle(
		Size size,
		Buffer<float> output
	) {
		float cx = (float)size.Columns / 2.0f;
		float cy = (float)size.Rows / 2.0f;
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
				output[r][c] = distance;
			}
		}

		var swap = new Buffer<float>( output.Size );
		// Push the edge from the corner to the mid-point
		float target = output[0][size.Columns / 2];
		_noiseOperator.Threshold( output, 0.0f, 0.0f, target, target, swap );
		_noiseOperator.Normalize( swap, output );
		_noiseOperator.Invert( output, swap );
		output.CopyFrom( swap );

		return this;
	}

	public Buffer<float> Circle(
		Size size
	) {
		var output = new Buffer<float>( size );
		( this as INoiseMaskGenerator ).Circle( size, output );
		return output;
	}

}

