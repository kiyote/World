namespace Common.Players.Manager;

public interface IPlayerManager {

	Task<Player> CreateAsync(
		Id<Player> playerId,
		string name,
		CancellationToken cancellationToken
	);
}

