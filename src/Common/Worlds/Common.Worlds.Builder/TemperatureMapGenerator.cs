using Common.Buffers;
using Common.Buffers.Float;

namespace Common.Worlds.Builder;

internal class TemperatureMapGenerator : ITemperatureMapGenerator {

	private readonly IBufferFactory _bufferFactory;
	private readonly IFloatBufferOperators _bufferOperators;

	public TemperatureMapGenerator(
		IBufferFactory bufferFactory,
		IFloatBufferOperators bufferOperators
	) {
		_bufferFactory = bufferFactory;
		_bufferOperators = bufferOperators;
	}

	IBuffer<float> ITemperatureMapGenerator.Create(
		IBuffer<float> landform
	) {
		IBuffer<float> temperature = _bufferFactory.Create<float>( landform.Size );
		ApplyLatitude( temperature );
		ApplyAltitude( temperature, landform );

		// Box-blur
		IBuffer<float> smoothed = _bufferFactory.Create<float>( temperature.Size );
		_bufferOperators.HorizonalBlur( temperature, 20, smoothed );
		_bufferOperators.VerticalBlur( smoothed, 10, temperature );

		return temperature;
	}

	private static void ApplyLatitude(
		IBuffer<float> temperature
	) {
		for( int r = 0; r < temperature.Size.Rows; r++ ) {
			float latitude = Math.Abs( r - ( temperature.Size.Rows / 2 ) );
			latitude = 1.0f - (latitude / (temperature.Size.Rows / 2));

			for( int c = 0; c < temperature.Size.Columns - 1; c++ ) {
				temperature[c, r] = latitude;
			}
		}
	}

	private static void ApplyAltitude(
		IBuffer<float> temperature,
		IBuffer<float> landform
	) {
		for( int r = 0; r < temperature.Size.Rows; r++ ) {
			for( int c = 0; c < temperature.Size.Columns - 1; c++ ) {
				float altitude = landform[c, r];
				float temp = temperature[c, r];

				temp -= altitude;
				if (temp < 0.0f) {
					temp = 0.0f;
				}

				temperature[c, r] = temp;
			}
		}
	}
}
