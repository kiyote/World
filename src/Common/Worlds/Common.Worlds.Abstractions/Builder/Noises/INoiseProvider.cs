namespace Common.Worlds.Builder.Noises;

public interface INoiseProvider {

	float[,] Generate(
		long seed,
		int rows,
		int columns,
		float frequency
	);
}

