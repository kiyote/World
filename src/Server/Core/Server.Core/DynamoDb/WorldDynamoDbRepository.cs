using InjectableAWS;
using InjectableAWS.Repository;

namespace Server.Core.DynamoDb;

public interface IWorldDynamoDbRepository: IDynamoDbRepository<WorldDynamoDbRepositoryOptions> {
}

public class WorldDynamoDbRepository : DynamoDbRepository<WorldDynamoDbRepositoryOptions>, IWorldDynamoDbRepository {
	public WorldDynamoDbRepository(
		WorldDynamoDbRepositoryOptions options,
		DynamoDbContext<WorldDynamoDbRepositoryOptions> context
	) : base( options, context ) {
	}
}
