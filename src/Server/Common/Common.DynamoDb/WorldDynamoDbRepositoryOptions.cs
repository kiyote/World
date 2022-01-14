using InjectableAWS.Repository;

namespace Common.DynamoDb;

public record WorldDynamoDbRepositoryOptions(string TableName): DynamoDbRepositoryOptions(TableName);
