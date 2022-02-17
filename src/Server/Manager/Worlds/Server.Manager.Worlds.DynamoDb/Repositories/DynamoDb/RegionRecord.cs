using Amazon.DynamoDBv2.DataModel;
using Common.Manager.Worlds;

namespace Server.Manager.Worlds.Repositories.DynamoDb;

[System.Diagnostics.CodeAnalysis.SuppressMessage( "Performance", "CA1812:An internal (assembly-level) type is never instantiated.", Justification = "TEMPORARY" )]
internal class RegionRecord {

	public RegionRecord() {
		WorldId = "";
		RegionId = "";
		Name = "";
		CreatedOn = DateTime.UtcNow;
	}

	public RegionRecord(
		Id<World> worldId,
		Id<Region> regionId,
		string name,
		DateTime createdOn
	) {
		WorldId = worldId.Value;
		RegionId = regionId.Value;
		Name = name;
		CreatedOn = createdOn;
	}

	[DynamoDBHashKey( "PK" )]
	public string WorldId { get; set; }

	[DynamoDBRangeKey( "SK" )]
	public string RegionId { get; set; }

	[DynamoDBProperty]
	public string Name { get; set; }

	[DynamoDBProperty]
	public DateTime CreatedOn { get; set; }

}
