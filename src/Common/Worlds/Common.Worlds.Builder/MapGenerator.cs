using Common.Buffers;
using Common.Buffers.Float;

namespace Common.Worlds.Builder;

internal sealed class MapGenerator : IMapGenerator {

	private readonly IBufferFactory _bufferFactory;
	private readonly IFloatBufferOperators _floatBufferOperators;
	private readonly IBufferOperator _bufferOperator;

	public MapGenerator(
		IBufferFactory bufferFactory,
		IFloatBufferOperators floatBufferOperators,
		IBufferOperator bufferOperator
	) {
		_bufferFactory = bufferFactory;
		_floatBufferOperators = floatBufferOperators;
		_bufferOperator = bufferOperator;
	}

	WorldMaps IMapGenerator.Create(
		LandformMaps landformMaps
	) {
		Size size = landformMaps.Height.Size;

		float[] ranges = new float[] { 0.0f, 0.3f, 0.6f, 0.9f, float.MaxValue };

		IBuffer<float> quantized = _bufferFactory.Create<float>( size );
		_floatBufferOperators.Quantize( landformMaps.Height, ranges, quantized );
		IBuffer<TileTerrain> terrain = _bufferFactory.Create<TileTerrain>( size );
		_bufferOperator.Perform(
			quantized,
			( int c, int r, float floatValue ) => {
				if( landformMaps.FreshWater[c, r] ) {
					return TileTerrain.Lake;
				}

				int value = (int)floatValue;
				if( value == 0 ) {
					return TileTerrain.Ocean;
				} else if( value == 1 ) {
					return TileTerrain.Coast;
				} else if( value == 2 ) {
					return TileTerrain.Plain;
				} else if( value == 3 ) {
					return TileTerrain.Hill;
				} else if( value == 4 ) {
					return TileTerrain.Mountain;
				} else {
					throw new InvalidOperationException();
				}
			},
			terrain
		);

		IBuffer<TileFeature> feature = _bufferFactory.Create<TileFeature>( size );
		/*
		_bufferOperator.Perform(
			landformMaps.Moisture,
			( int c, int r, float moistureLevel ) => {
				float temperature = landformMaps.Temperature[c, r];
				if( terrain[c, r] == TileTerrain.Plain ) {
					if( temperature > 0.4f
						&& moistureLevel > 0.4f
					) {
						return TileFeature.Forest;
					}
				}
				return TileFeature.None;
			},
			feature
		);
		*/


		return new WorldMaps( terrain, feature );
	}
}

