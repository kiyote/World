using Common.Buffers;

namespace Common.Worlds.Builder;

public interface ILandformGenerator {
	IBuffer<float> Create(
		long seed,
		Size size,
		INeighbourLocator neighbourLocator
	);
}

