using Common.Buffers;
using Common.Buffers.Float;

namespace Common.Worlds.Builder;

internal sealed class TerrainMapGenerator : ITerrainMapGenerator {

	private readonly IBufferFactory _bufferFactory;
	private readonly IFloatBufferOperators _floatBufferOperators;

	public TerrainMapGenerator(
		IBufferFactory bufferFactory,
		IFloatBufferOperators floatBufferOperators
	) {
		_bufferFactory = bufferFactory;
		_floatBufferOperators = floatBufferOperators;
	}

	IBuffer<TileTerrain> ITerrainMapGenerator.Create(
		IBuffer<float> landform,
		IBuffer<float> temperature,
		IBuffer<float> precipitation,
		IBuffer<float> airFlow
	) {
		IBuffer<TileTerrain> result = _bufferFactory.Create<TileTerrain>( landform.Size );
		return result;
	}
}
