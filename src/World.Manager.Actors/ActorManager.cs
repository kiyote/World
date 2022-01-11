namespace World.Manager.Actors;

using World.Repository.Actors;

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
		Id<Actor> id,
		string name,
		CancellationToken cancellationToken
	) {
		return await _actorRepository
			.CreateAsync( id, name, cancellationToken )
			.ConfigureAwait( false );
	}
}
