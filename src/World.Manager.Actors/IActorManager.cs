namespace World.Manager.Actors;

public interface IActorManager {
	Task<Actor> CreateAsync(
		Id<Actor> id,
		string name,
		CancellationToken cancellationToken
	);
}
