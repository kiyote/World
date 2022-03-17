namespace Common.Worlds.Builder;

public interface ILandformGenerator {
	ILandformGenerator Create(
		long seed,
		Size size,
		INeighbourLocator neighbourLocator,
		Buffer<float> probabilityMask,
		Buffer<float> output
	);

	Buffer<float> Create(
		long seed,
		Size size,
		INeighbourLocator neighbourLocator,
		Buffer<float> probabilityMask
	);
}

