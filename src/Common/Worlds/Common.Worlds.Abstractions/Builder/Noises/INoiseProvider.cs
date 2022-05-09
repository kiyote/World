using Common.Buffers;

namespace Common.Worlds.Builder.Noises;

public interface INoiseProvider {

	void Random(
		long seed,
		float frequency,
		IBuffer<float> output
	);
}

