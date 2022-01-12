﻿namespace Manager.Actors.Repositories;

public interface IActorRepository
{
	Task<Actor> CreateAsync(
		Id<World> worldId,
		Id<Actor> actorId,
		string name,
		CancellationToken cancellationToken
	);

	Task<Actor> GetByIdAsync(
		Id<World> worldId,
		Id<Actor> actorId,
		CancellationToken cancellationToken
	);
}
