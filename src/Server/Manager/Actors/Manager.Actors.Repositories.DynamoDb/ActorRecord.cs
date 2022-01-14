using Amazon.DynamoDBv2.DataModel;

namespace Manager.Actors.Repositories.DynamoDb;

internal sealed class ActorRecord {

	public ActorRecord() {
		WorldId = "";
		ActorId = "";
		Name = "";
	}

	[DynamoDBHashKey("PK")]
	public string WorldId { get; set; }

	[DynamoDBRangeKey("SK")]
	public string ActorId { get; set; }

	[DynamoDBProperty]
	public string Name { get; set; }

}
