using Common.DynamoDb;

namespace Manager.Actors.Repositories.DynamoDb;

[System.Diagnostics.CodeAnalysis.SuppressMessage( "Performance", "CA1812:An internal (assembly-level) type is never instantiated.", Justification = "This class is instantiated via DI." )]
internal sealed class DynamoDbActorRepository : IActorRepository {

	private readonly WorldDynamoDbRepository _db;

	public DynamoDbActorRepository(
		WorldDynamoDbRepository db
	) {
		_db = db;
	}

	async Task<Actor> IActorRepository.CreateAsync(
		Id<World> worldId,
		Id<Actor> actorId,
		string name,
		DateTime createdOn,
		CancellationToken cancellationToken
	) {
		ActorRecord record = new ActorRecord(
			worldId,
			actorId,
			name,
			createdOn.ToUniversalTime()
		);
		await _db
			.CreateAsync(
				record,
				cancellationToken
			).ConfigureAwait( false );

		return ToActor( record );
	}

	async Task<Actor> IActorRepository.GetByIdAsync(
		Id<World> worldId,
		Id<Actor> actorId,
		CancellationToken cancellationToken
	) {
		ActorRecord record = await _db
			.LoadAsync<ActorRecord>(
				worldId.ToString(),
				actorId.ToString(),
				cancellationToken
			).ConfigureAwait( false );

		return ToActor( record );
	}

	private static Actor ToActor(
		ActorRecord record
	) {
		return new Actor(
			new Id<Actor>( record.ActorId ),
			record.Name,
			record.CreatedOn
		);
	}
}
