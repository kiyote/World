namespace Common.Manager.Players.Repositories;

public interface IPlayerRepository
{
	Task<Player> CreateAsync(
		Id<Player> playerId,
		string name,
		DateTime createdOn,
		CancellationToken cancellationToken
	);
}
