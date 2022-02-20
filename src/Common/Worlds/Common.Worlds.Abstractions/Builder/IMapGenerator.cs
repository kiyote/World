namespace Common.Worlds.Builder;

public interface IMapGenerator {
	TileTerrain[,] GenerateTerrain(
		string seed,
		int rows,
		int columns
	);
}
