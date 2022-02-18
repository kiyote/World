using Common.Worlds;
using Common.Worlds.Manager.Repositories;
using Server.Core.DynamoDb;

namespace Server.Worlds.Manager.Repositories.DynamoDb;

[System.Diagnostics.CodeAnalysis.SuppressMessage( "Performance", "CA1812:An internal (assembly-level) type is never instantiated.", Justification = "This class is instantiated via DI." )]
internal sealed class DynamoDbWorldRepository : IWorldRepository {

	private readonly IWorldDynamoDbRepository _db;

	public DynamoDbWorldRepository(
		IWorldDynamoDbRepository db
	) {
		_db = db;
	}

	async Task<World> IWorldRepository.CreateAsync(
		Id<World> worldId,
		string name,
		string seed,
		int rows,
		int columns,
		DateTime createdOn,
		CancellationToken cancellationToken
	) {
		WorldRecord record = new WorldRecord(
			worldId,
			name,
			seed,
			rows,
			columns,
			createdOn.ToUniversalTime()
		);

		await _db.CreateAsync( record, cancellationToken ).ConfigureAwait( false );

		return new World(
			worldId,
			name,
			seed,
			rows,
			columns,
			createdOn
		);
	}

	async Task<World?> IWorldRepository.GetByIdAsync(
		Id<World> worldId,
		CancellationToken cancellationToken
	) {
		WorldRecord? record = await _db.LoadAsync<WorldRecord?>(
			worldId.Value,
			worldId.Value,
			cancellationToken
		).ConfigureAwait( false );

		if (record is null) {
			return null;
		}

		return new World(
			worldId,
			record.Name,
			record.Seed,
			record.Rows,
			record.Columns,
			record.CreatedOn
		);
		
	}
}
