namespace Manager.Worlds.Repositories;

public interface IWorldRepository
{
	Task<World> Create(
		Id<World> worldId,
		string name,
		DateTime createdOn,
		CancellationToken cancellationToken
	);
}
