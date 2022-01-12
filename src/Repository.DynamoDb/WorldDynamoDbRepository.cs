namespace Repository.DynamoDb;

using InjectableAWS;
using InjectableAWS.Repository;

public class WorldDynamoDbRepository : DynamoDbRepository<WorldDynamoDbRepositoryOptions> {
	public WorldDynamoDbRepository(
		WorldDynamoDbRepositoryOptions options,
		DynamoDbContext<WorldDynamoDbRepositoryOptions> context
	) : base( options, context ) {
	}
}
