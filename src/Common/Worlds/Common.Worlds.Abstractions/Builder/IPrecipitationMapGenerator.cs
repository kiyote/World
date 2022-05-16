using Common.Buffers;

namespace Common.Worlds.Builder;

public interface IPrecipitationMapGenerator {
	IBuffer<float> Create(
		IBuffer<float> landform
	);
}
