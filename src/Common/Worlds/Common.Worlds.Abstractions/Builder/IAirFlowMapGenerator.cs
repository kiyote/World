using Common.Buffers;

namespace Common.Worlds.Builder;

public interface IAirFlowMapGenerator {

	IBuffer<float> Create(
		IBuffer<float> landform
	);
}
