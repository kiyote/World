namespace Repository.DynamoDb;

using InjectableAWS;

public class WorldDynamoDbRepository : DynamoDbRepository<WorldDynamoDbRepositoryOptions> {
	public WorldDynamoDbRepository(
		WorldDynamoDbRepositoryOptions options,
		DynamoDbContext<WorldDynamoDbRepositoryOptions> context
	) : base( options, context ) {
	}
}
