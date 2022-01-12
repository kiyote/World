namespace Manager.Worlds.Repositories.DynamoDb;

using Repository.DynamoDb;
using Amazon.DynamoDBv2.DataModel;

[System.Diagnostics.CodeAnalysis.SuppressMessage( "Performance", "CA1812:An internal (assembly-level) type is never instantiated.", Justification = "TEMPORARY" )]
internal class WorldRecord {

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
