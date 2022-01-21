namespace Manager.Worlds;


public interface IWorldManager {
	Task<World> CreateAsync(
		Id<World> worldId,
		string name,
		CancellationToken cancellationToken
	);
}
