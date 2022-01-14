using Amazon.DynamoDBv2.DataModel;

namespace Manager.Actors.Repositories.DynamoDb;

internal sealed class ActorRecord {

	public ActorRecord() {
		WorldId = "";
		ActorId = "";
		Name = "";
		CreatedOn = DateTime.UtcNow;
	}

	public ActorRecord(
		Id<World> worldId,
		Id<Actor> actorId,
		string name,
		DateTime createdOn
	) {
		WorldId = worldId.Value;
		ActorId = actorId.Value;
		Name = name;
		CreatedOn = createdOn.ToUniversalTime();
	}

	[DynamoDBHashKey("PK")]
	public string WorldId { get; set; }

	[DynamoDBRangeKey("SK")]
	public string ActorId { get; set; }

	[DynamoDBProperty]
	public string Name { get; set; }

	[DynamoDBProperty]
	public DateTime CreatedOn { get; set; }

}
