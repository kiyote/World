namespace Common.Worlds.Builder;

public interface ILandformMapGenerator {

	/// <summary>
	/// The maps used for map generation.
	/// </summary>
	/// <param name="seed">Initializes the world generation.</param>
	/// <param name="size">The dimensions of the resulting map.</param>
	/// <param name="neighbourLocator">The method by which a cells neighbours are determined.</param>
	/// <returns>Returns an <c>LandformMaps</c> containing the <c>IBuffer<float></c>
	/// maps of the landform.</returns>
	LandformMaps Create(
		long seed,
		Size size,
		INeighbourLocator neighbourLocator
	);
}

