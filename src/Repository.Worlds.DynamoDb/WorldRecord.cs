namespace Repository.Worlds.DynamoDb;

using Repository.DynamoDb;
using Amazon.DynamoDBv2.DataModel;

#pragma warning disable CA1812
internal class WorldRecord {
#pragma warning restore CA1812

	public WorldRecord() {
		WorldId = "";
		Name = "";
	}

	[DynamoDBHashKey( "PK" )]
	[DynamoDBRangeKey( "SK" )]
	public string WorldId { get; set; }

	[DynamoDBProperty]
	public string Name { get; set; }

}
