using InjectableAWS.Repository;

namespace Server.Common.DynamoDb;

public record WorldDynamoDbRepositoryOptions( string TableName, string IndexName )
	: DynamoDbRepositoryOptions( TableName, IndexName );
