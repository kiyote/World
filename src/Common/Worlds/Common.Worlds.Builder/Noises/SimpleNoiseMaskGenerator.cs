using Common.Buffer;
using Common.Buffer.Unit;

namespace Common.Worlds.Builder.Noises;

internal sealed class SimpleNoiseMaskGenerator : INoiseMaskGenerator {

	private readonly IBufferFactory _bufferFactory;
	private readonly IUnitBufferFilterOperators _filterOperators;
	private readonly IUnitBufferClippingOperators _clippingOperators;

	public SimpleNoiseMaskGenerator(
		IBufferFactory bufferFactory,
		IUnitBufferFilterOperators filterOperators,
		IUnitBufferClippingOperators clippingOperators
	) {
		_bufferFactory = bufferFactory;
		_filterOperators = filterOperators;
		_clippingOperators = clippingOperators;
	}

	INoiseMaskGenerator INoiseMaskGenerator.Circle(
		Size size,
		IBuffer<float> output
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

		IBuffer<float> swap = _bufferFactory.Create<float>( output.Size );
		// Push the edge from the corner to the mid-point
		float target = output[0][size.Columns / 2];
		_clippingOperators.Threshold( output, 0.0f, 0.0f, target, target, swap );
		_clippingOperators.Normalize( swap, output );
		_filterOperators.Invert( output, swap );
		output.CopyFrom( swap );

		return this;
	}

	public IBuffer<float> Circle(
		Size size
	) {
		IBuffer<float> output = _bufferFactory.Create<float>( size );
		( this as INoiseMaskGenerator ).Circle( size, output );
		return output;
	}

}

