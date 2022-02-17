using Amazon.DynamoDBv2.DataModel;
using Common.Manager.Worlds;

namespace Server.Manager.Worlds.Repositories.DynamoDb;

[System.Diagnostics.CodeAnalysis.SuppressMessage( "Performance", "CA1812:An internal (assembly-level) type is never instantiated.", Justification = "TEMPORARY" )]
internal class WorldRecord {

	public WorldRecord() {
		WorldId = "";
		Name = "";
		Seed = "";
		Rows = 0;
		Columns = 0;
		CreatedOn = DateTime.UtcNow;
	}

	public WorldRecord(
		Id<World> worldId,
		string name,
		string seed,
		int rows,
		int columns,
		DateTime createdOn
	) {
		WorldId = worldId.Value;
		Name = name;
		Seed = seed;
		Rows = rows;
		Columns = columns;
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
	public int Rows { get; set; }

	[DynamoDBProperty]
	public int Columns { get; set; }

	[DynamoDBProperty]
	public DateTime CreatedOn { get; set; }

}
