using Common.Buffers;

namespace Common.Worlds.Builder;

public interface ITemperatureMapGenerator {
	IBuffer<float> Create(
		IBuffer<float> landform
	);
}
