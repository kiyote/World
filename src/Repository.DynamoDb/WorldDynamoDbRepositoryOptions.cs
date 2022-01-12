namespace Repository.DynamoDb;

using InjectableAWS.Repository;

public record WorldDynamoDbRepositoryOptions(string TableName): DynamoDbRepositoryOptions(TableName);
