namespace Common.Worlds.Builder;

public interface IMapGenerator {
	TileTerrain[,] GenerateTerrain(
		string seed,
		Size size
	);

	TileTerrain[,] GenerateTerrain(
		long seed,
		Size size
	);
}
