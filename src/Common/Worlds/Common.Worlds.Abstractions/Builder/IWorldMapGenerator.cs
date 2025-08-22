namespace Common.Worlds.Builder;

public interface IWorldMapGenerator {
	Task<WorldMaps> CreateAsync(
		long seed,
		ISize size,
		INeighbourLocator neighbourLocator,
		CancellationToken cancellationToken
	);
}
