using Common.Buffers;

namespace Common.Worlds.Builder;

public interface ILandformMapGenerator {

	/// <summary>
	/// Generates a heightmap of the specified <paramref name="size"/> as a
	/// buffer of values between 0 and 1.
	/// </summary>
	/// <param name="seed">Initializes the world generation.</param>
	/// <param name="size">The dimensions of the resulting heightmap.</param>
	/// <param name="neighbourLocator">The method by which a cells neighbours are determined.</param>
	/// <returns>Returns an <c>IBuffer<float></c> containing the heightmap
	/// as a set of values between 0 and 1.</returns>
	IBuffer<float> Create(
		long seed,
		Size size,
		INeighbourLocator neighbourLocator
	);
}

