using Common.Manager.Players.Repositories;

namespace Common.Manager.Players;

[System.Diagnostics.CodeAnalysis.SuppressMessage( "Performance", "CA1812:An internal (assembly-level) type is never instantiated.", Justification = "This class is instantiated via DI." )]
internal sealed class PlayerManager : IPlayerManager {

	private readonly IPlayerRepository _playerRepository;

	public PlayerManager(
		IPlayerRepository playerRepository
	) {
		_playerRepository = playerRepository;
	}

	Task<Player> IPlayerManager.CreateAsync(
		Id<Player> playerId,
		string name,
		CancellationToken cancellationToken
	) {
		return _playerRepository.CreateAsync(
			playerId,
			name,
			DateTime.UtcNow,
			cancellationToken
		);
	}
}
