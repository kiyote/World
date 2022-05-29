namespace Common.Worlds.Builder;

public interface IWorldMapGenerator {
	WorldMaps Create(
		long seed,
		Size size,
		INeighbourLocator neighbourLocator
	);
}
