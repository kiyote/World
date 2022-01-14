using InjectableAWS;
using InjectableAWS.Repository;

namespace Common.DynamoDb;

public class WorldDynamoDbRepository : DynamoDbRepository<WorldDynamoDbRepositoryOptions> {
	public WorldDynamoDbRepository(
		WorldDynamoDbRepositoryOptions options,
		DynamoDbContext<WorldDynamoDbRepositoryOptions> context
	) : base( options, context ) {
	}
}
