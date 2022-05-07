using Common.Buffer;

namespace Common.Worlds.Builder;

public interface IMapGenerator {

	IBuffer<TileTerrain> GenerateTerrain(
		string seed,
		Size size
	);

	IBuffer<TileTerrain> GenerateTerrain(
		long seed,
		Size size
	);
}
