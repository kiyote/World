using InjectableAWS.Repository;

namespace Common.DynamoDb;

public record WorldDynamoDbRepositoryOptions( string TableName, string IndexName )
	: DynamoDbRepositoryOptions( TableName, IndexName );
