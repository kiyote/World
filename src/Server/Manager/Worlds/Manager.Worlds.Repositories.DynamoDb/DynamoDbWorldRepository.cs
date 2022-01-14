using Common.DynamoDb;
using Common.Model.Worlds;

namespace Manager.Worlds.Repositories.DynamoDb;

[System.Diagnostics.CodeAnalysis.SuppressMessage( "Performance", "CA1812:An internal (assembly-level) type is never instantiated.", Justification = "This class is instantiated via DI." )]
internal class DynamoDbWorldRepository : IWorldRepository {

	private readonly WorldDynamoDbRepository _db;

	public DynamoDbWorldRepository(
		WorldDynamoDbRepository db
	) {
		_db = db;
	}

	async Task<World> IWorldRepository.Create(
		Id<World> worldId,
		string name,
		DateTime createdOn,
		CancellationToken cancellationToken
	) {
		WorldRecord record = new WorldRecord(
			worldId,
			name,
			createdOn.ToUniversalTime()
		);

		await _db.CreateAsync( record, cancellationToken ).ConfigureAwait( false );

		return new World(
			worldId,
			name,
			createdOn
		);
	}
}
