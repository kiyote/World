namespace Manager.Worlds.Repositories;

public interface IWorldRepository
{
	Task<World> Create(
		Id<World> worldId,
		string name,
		CancellationToken cancellationToken
	);
}
