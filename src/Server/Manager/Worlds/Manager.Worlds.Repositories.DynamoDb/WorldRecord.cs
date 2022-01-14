using Amazon.DynamoDBv2.DataModel;

namespace Manager.Worlds.Repositories.DynamoDb;

[System.Diagnostics.CodeAnalysis.SuppressMessage( "Performance", "CA1812:An internal (assembly-level) type is never instantiated.", Justification = "TEMPORARY" )]
internal class WorldRecord {

	public WorldRecord() {
		WorldId = "";
		Name = "";
	}

	public WorldRecord(
		Id<World> worldId,
		string name
	) {
		WorldId = worldId.Value;
		Name = name;
	}

	[DynamoDBHashKey( "PK" )]
	[DynamoDBRangeKey( "SK" )]
	public string WorldId { get; set; }

	[DynamoDBProperty]
	public string Name { get; set; }

}
