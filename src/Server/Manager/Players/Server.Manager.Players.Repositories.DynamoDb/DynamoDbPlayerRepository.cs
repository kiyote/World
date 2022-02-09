using Common.Manager.Players.Repositories;
using Server.Common.DynamoDb;

namespace Server.Manager.Players.Repositories.DynamoDb;

[System.Diagnostics.CodeAnalysis.SuppressMessage( "Performance", "CA1812:An internal (assembly-level) type is never instantiated.", Justification = "This class is instantiated via DI." )]
internal sealed class DynamoDbPlayerRepository : IPlayerRepository {

	private readonly IWorldDynamoDbRepository _db;

	public DynamoDbPlayerRepository(
		IWorldDynamoDbRepository db
	) {
		_db = db;
	}

	async Task<Player> IPlayerRepository.CreateAsync(
		Id<Player> playerId,
		string name,
		DateTime createdOn,
		CancellationToken cancellationToken
	) {
		PlayerRecord record = new PlayerRecord(
			playerId,
			name,
			createdOn.ToUniversalTime()
		);

		await _db.CreateAsync( record, cancellationToken ).ConfigureAwait( false );

		return new Player(
			playerId,
			name,
			createdOn
		);
	}
}
