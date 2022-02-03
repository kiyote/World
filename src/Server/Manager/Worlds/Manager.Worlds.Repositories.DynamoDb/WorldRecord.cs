using Amazon.DynamoDBv2.DataModel;

namespace Manager.Worlds.Repositories.DynamoDb;

[System.Diagnostics.CodeAnalysis.SuppressMessage( "Performance", "CA1812:An internal (assembly-level) type is never instantiated.", Justification = "TEMPORARY" )]
internal class WorldRecord {

	public WorldRecord() {
		WorldId = "";
		Name = "";
		Seed = "";
		CreatedOn = DateTime.UtcNow;
	}

	public WorldRecord(
		Id<World> worldId,
		string name,
		string seed,
		DateTime createdOn
	) {
		WorldId = worldId.Value;
		Name = name;
		Seed = seed;
		CreatedOn = createdOn;
	}

	[DynamoDBHashKey( "PK" )]
	[DynamoDBRangeKey( "SK" )]
	public string WorldId { get; set; }

	[DynamoDBProperty]
	public string Name { get; set; }

	[DynamoDBProperty]
	public string Seed { get; set; }

	[DynamoDBProperty]
	public DateTime CreatedOn { get; set; }

}
