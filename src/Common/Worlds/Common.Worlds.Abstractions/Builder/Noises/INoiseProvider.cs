namespace Common.Worlds.Builder.Noises;

public interface INoiseProvider {

	Buffer<float> Random(
		long seed,
		int rows,
		int columns,
		float frequency
	);
}

