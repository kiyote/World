namespace Repository.DynamoDb;

public record WorldDynamoDbRepositoryOptions(string TableName): DynamoDbRepositoryOptions(TableName);
