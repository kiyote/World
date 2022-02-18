namespace Common.Worlds.Builder;

public interface INoiseProvider {

	float[,] Generate(
		long seed,
		int rows,
		int columns,
		float frequency
	);
}

