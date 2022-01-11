namespace Repository.Actors.DynamoDb;

using Repository.DynamoDb;

#pragma warning disable CA1812
internal sealed class DynamoDbActorRepository : IActorRepository {
#pragma warning restore CA1812

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
		CancellationToken cancellationToken
	) {
		ActorRecord record = new ActorRecord() {
			WorldId = worldId.ToString(),
			ActorId = actorId.ToString(),
			Name = name
		};
		await _db
			.CreateAsync(
				record,
				cancellationToken
			).ConfigureAwait( false );

		return ToActor( record );
	}

	async Task<Actor> IActorRepository.GetByIdAsync(
		Id<Actor> id,
		CancellationToken cancellationToken
	) {
		ActorRecord record = await _db
			.LoadAsync<ActorRecord>(
				id.ToString(),
				id.ToString(),
				cancellationToken
			).ConfigureAwait( false );

		return ToActor( record );
	}

	private static Actor ToActor(
		ActorRecord record
	) {
		return new Actor(
			new Id<Actor>( record.ActorId ),
			record.Name
		);
	}
}
