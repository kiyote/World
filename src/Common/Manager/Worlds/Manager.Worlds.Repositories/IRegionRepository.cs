namespace Manager.Worlds.Repositories;

public interface IRegionRepository {
	Task<Region> CreateAsync(
		Id<World> worldId,
		Id<Region> regionId,
		string name,
		DateTime createdOn,
		CancellationToken cancellationToken
	);

	Task<Region?> GetByIdAsync(
		Id<World> worldId,
		Id<Region> regionId,
		CancellationToken cancellationToken
	);
}
