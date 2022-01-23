using Manager.Worlds.Repositories;

namespace Manager.Worlds;

[System.Diagnostics.CodeAnalysis.SuppressMessage( "Performance", "CA1812:An internal (assembly-level) type is never instantiated.", Justification = "This class is instantiated via DI." )]
internal sealed class WorldManager : IWorldManager {

	private readonly IWorldRepository _worldRepository;
	private readonly IRegionRepository _regionRepository;

	public WorldManager(
		IWorldRepository worldRepository,
		IRegionRepository regionRepository
	) {
		_worldRepository = worldRepository;
		_regionRepository = regionRepository;
	}

	Task<World> IWorldManager.CreateWorldAsync(
		Id<World> worldId,
		string name,
		CancellationToken cancellationToken
	) {
		return _worldRepository.CreateAsync(
			worldId,
			name,
			DateTime.UtcNow,
			cancellationToken );
	}

	Task<Region> IWorldManager.CreateRegionAsync(
		Id<World> worldId,
		Id<Region> regionId,
		string name,
		CancellationToken cancellationToken
	) {
		return _regionRepository.CreateAsync(
			worldId,
			regionId,
			name,
			Array.Empty<Id<RegionChunk>>(),
			DateTime.UtcNow,
			cancellationToken );
	}

	Task<IEnumerable<Tile>> IWorldManager.GetRegionChunkTilesAsync(
		Id<RegionChunk> regionChunkId,
		CancellationToken cancellationToken
	) {
		throw new NotImplementedException();
	}
}

