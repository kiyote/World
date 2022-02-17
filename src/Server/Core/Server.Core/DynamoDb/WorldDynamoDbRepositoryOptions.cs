using InjectableAWS.Repository;

namespace Server.Core.DynamoDb;

public record WorldDynamoDbRepositoryOptions( string TableName, string IndexName )
	: DynamoDbRepositoryOptions( TableName, IndexName );
