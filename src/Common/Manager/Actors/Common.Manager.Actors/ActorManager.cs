using Common.Manager.Actors.Repositories;
using Common.Manager.Worlds;

namespace Common.Manager.Actors;

[System.Diagnostics.CodeAnalysis.SuppressMessage( "Performance", "CA1812:An internal (assembly-level) type is never instantiated.", Justification = "This class is instantiated via DI." )]

internal class ActorManager : IActorManager {

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
				DateTime.UtcNow,
				cancellationToken
			)
			.ConfigureAwait( false );
	}

	async Task<Actor?> IActorManager.GetByIdAsync(
		Id<World> worldId,
		Id<Actor> actorId,
		CancellationToken cancellationToken
	) {
		return await _actorRepository
			.GetByIdAsync(
				worldId,
				actorId,
				cancellationToken
			)
			.ConfigureAwait( false );
	}
}
