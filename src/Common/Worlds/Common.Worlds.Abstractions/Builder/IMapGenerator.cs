namespace Common.Worlds.Builder;

public interface IMapGenerator {

	Buffer<TileTerrain> GenerateTerrain(
		string seed,
		Size size
	);

	Buffer<TileTerrain> GenerateTerrain(
		long seed,
		Size size
	);
}
