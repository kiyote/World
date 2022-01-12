namespace Manager.Actors;

public interface IActorManager {
	Task<Actor> CreateAsync(
		Id<World> worldId,
		Id<Actor> actorId,
		string name,
		CancellationToken cancellationToken
	);
}
