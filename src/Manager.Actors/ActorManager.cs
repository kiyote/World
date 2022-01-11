namespace Manager.Actors;

using Repository.Actors;

#pragma warning disable CA1812
internal class ActorManager : IActorManager {
#pragma warning restore CA1812

	private readonly IActorRepository _actorRepository;

	public ActorManager(
		IActorRepository actorRepository
	) {
		_actorRepository = actorRepository;
	}

	async Task<Actor> IActorManager.CreateAsync(
		Id<World> worldId,
		Id<Actor> actorId,
		string name,
		CancellationToken cancellationToken
	) {
		return await _actorRepository
			.CreateAsync(
				worldId,
				actorId,
				name,
				cancellationToken
			)
			.ConfigureAwait( false );
	}
}
