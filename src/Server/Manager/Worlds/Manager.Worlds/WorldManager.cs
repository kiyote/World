using Manager.Worlds.Repositories;

namespace Manager.Worlds;

[System.Diagnostics.CodeAnalysis.SuppressMessage( "Performance", "CA1812:An internal (assembly-level) type is never instantiated.", Justification = "This class is instantiated via DI." )]
internal sealed class WorldManager : IWorldManager {

	private readonly IWorldRepository _worldRepository;

	public WorldManager(
		IWorldRepository worldRepository
	) {
		_worldRepository = worldRepository;
	}

	Task<World> IWorldManager.CreateAsync(
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
}

