using Common.Buffers;
using Common.Buffers.Float;

namespace Common.Worlds.Builder;

internal sealed class AirFlowMapGenerator : IAirFlowMapGenerator {

	private readonly IRandom _random;
	private readonly IBufferFactory _bufferFactory;
	private readonly IFloatBufferOperators _bufferOperators;

	public AirFlowMapGenerator(
		IRandom random,
		IBufferFactory bufferFactory,
		IFloatBufferOperators bufferOperators
	) {
		_random = random;
		_bufferFactory = bufferFactory;
		_bufferOperators = bufferOperators;
	}

	IBuffer<float> IAirFlowMapGenerator.Create(
		IBuffer<float> landform
	) {
		IBuffer<float> airflow = _bufferFactory.Create( landform.Size, 0.5f );

		float streamThickness = landform.Size.Rows / 10;
		int northStream = _random.NextInt( landform.Size.Rows / 2 );
		int southStream = _random.NextInt( landform.Size.Rows / 2 ) + ( landform.Size.Rows / 2 );

		for( int i = 0; i < streamThickness; i++ ) {
			float intensity = Math.Abs( i - ( streamThickness / 2 ) );
			intensity = 1.0f - ( intensity / ( streamThickness / 2.0f ) );

			intensity /= 2;
			if( northStream + i < airflow.Size.Rows ) {
				for( int c = 0; c < airflow.Size.Columns; c++ ) {
					airflow[c, northStream + i] += intensity;
				}
			}

			if( southStream + i < airflow.Size.Rows ) {
				for( int c = 0; c < airflow.Size.Columns; c++ ) {
					airflow[c, southStream + i] += intensity;
				}
			}
		}

		_bufferOperators.Subtract( airflow, landform, airflow );
		_bufferOperators.Normalize( airflow, 0.0f, 1.0f );


		return airflow;
	}
}

