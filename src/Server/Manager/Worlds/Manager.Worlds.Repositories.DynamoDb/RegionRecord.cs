using Amazon.DynamoDBv2.DataModel;

namespace Manager.Worlds.Repositories.DynamoDb;

[System.Diagnostics.CodeAnalysis.SuppressMessage( "Performance", "CA1812:An internal (assembly-level) type is never instantiated.", Justification = "TEMPORARY" )]
internal class RegionRecord {

	public RegionRecord() {
		WorldId = "";
		RegionId = "";
		Name = "";
		Chunks = Array.Empty<Id<RegionChunk>>();
		CreatedOn = DateTime.UtcNow;
	}

	public RegionRecord(
		Id<World> worldId,
		Id<Region> regionId,
		string name,
		IEnumerable<Id<RegionChunk>> chunks,
		DateTime createdOn
	) {
		WorldId = worldId.Value;
		RegionId = regionId.Value;
		Name = name;
		Chunks = chunks.ToArray();
		CreatedOn = createdOn;
	}

	[DynamoDBHashKey( "PK" )]
	public string WorldId { get; set; }

	[DynamoDBRangeKey( "SK" )]
	public string RegionId { get; set; }

	[DynamoDBProperty]
	public Id<RegionChunk>[] Chunks { get; set; }

	[DynamoDBProperty]
	public string Name { get; set; }

	[DynamoDBProperty]
	public DateTime CreatedOn { get; set; }

}
