using InjectableAWS;

namespace World.Repository.Actors.DynamoDb;

#pragma warning disable CA1812
internal sealed class DynamoDbActorRepository : IActorRepository {
#pragma warning restore CA1812

	private readonly DynamoDbContext<DynamoDbActorRepositoryConfiguration> _db;
	private readonly DynamoDbActorRepositoryConfiguration _config;

	public DynamoDbActorRepository(
		DynamoDbContext<DynamoDbActorRepositoryConfiguration> db,
		DynamoDbActorRepositoryConfiguration config
	) {
		_db = db;
		_config = config;
	}

	async Task<Actor> IActorRepository.CreateAsync(
		Id<Actor> id,
		string name,
		CancellationToken cancellationToken
	) {
		await _db.Context
			.SaveAsync(
				new {
					id = id.Value,
					name = name
				},
				cancellationToken
			).ConfigureAwait( false );

		return new Actor( id, name );
	}
}
