using Common.Worlds;

namespace Common.Actors.Manager;

public interface IActorManager {
	Task<Actor> CreateAsync(
		Id<World> worldId,
		Id<Actor> actorId,
		string name,
		CancellationToken cancellationToken
	);

	Task<Actor?> GetByIdAsync(
		Id<World> worldId,
		Id<Actor> actorId,
		CancellationToken cancellationToken
	);
}
