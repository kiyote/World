namespace Common.Worlds.Manager.Repositories;

public interface IWorldRepository
{
	Task<World> CreateAsync(
		Id<World> worldId,
		string name,
		string seed,
		ISize size,
		DateTime createdOn,
		CancellationToken cancellationToken
	);

	Task<World?> GetByIdAsync(
		Id<World> worldId,
		CancellationToken cancellationToken
	);
}
