namespace Common.Players.Manager.Repositories;

public interface IPlayerRepository
{
	Task<Player> CreateAsync(
		Id<Player> playerId,
		string name,
		DateTime createdOn,
		CancellationToken cancellationToken
	);
}
