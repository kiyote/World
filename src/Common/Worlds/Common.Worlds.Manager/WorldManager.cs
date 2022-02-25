using Common.Worlds.Manager.Repositories;

namespace Common.Worlds.Manager;

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
		string seed,
		Size size,
		CancellationToken cancellationToken
	) {
		return _worldRepository.CreateAsync(
			worldId,
			name,
			seed,
			size,
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
			DateTime.UtcNow,
			cancellationToken );
	}

}

