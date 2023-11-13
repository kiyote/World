namespace Common.Worlds.Builder;

public interface IWorldMapGenerator {
	WorldMaps Create(
		long seed,
		ISize size,
		INeighbourLocator neighbourLocator
	);
}
