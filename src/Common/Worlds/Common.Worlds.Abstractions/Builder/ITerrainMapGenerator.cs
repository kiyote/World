using Common.Buffers;

namespace Common.Worlds.Builder;

public interface ITerrainMapGenerator {

	IBuffer<TileTerrain> Create(
		IBuffer<float> landform,
		IBuffer<float> temperature,
		IBuffer<float> precipitation,
		IBuffer<float> airFlow
	);
}
