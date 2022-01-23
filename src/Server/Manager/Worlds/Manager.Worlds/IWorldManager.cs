﻿namespace Manager.Worlds;


public interface IWorldManager {
	Task<World> CreateWorldAsync(
		Id<World> worldId,
		string name,
		CancellationToken cancellationToken
	);

	Task<Region> CreateRegionAsync(
		Id<World> worldId,
		Id<Region> regionId,
		string name,
		CancellationToken cancellationToken
	);

	Task<IEnumerable<Tile>> GetRegionChunkTilesAsync(
		Id<RegionChunk> regionChunkId,
		CancellationToken cancellationToken
	);
}
