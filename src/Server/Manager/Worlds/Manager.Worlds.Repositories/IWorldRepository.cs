﻿namespace Manager.Worlds.Repositories;

public interface IWorldRepository
{
	Task<World> CreateAsync(
		Id<World> worldId,
		string name,
		string seed,
		DateTime createdOn,
		CancellationToken cancellationToken
	);

	Task<World?> GetByIdAsync(
		Id<World> worldId,
		CancellationToken cancellationToken
	);
}
