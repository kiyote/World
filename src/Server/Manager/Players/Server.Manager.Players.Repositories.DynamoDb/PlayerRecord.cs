using Amazon.DynamoDBv2.DataModel;

namespace Server.Manager.Players.Repositories.DynamoDb;

internal sealed class PlayerRecord {

	public PlayerRecord() {
		PlayerId = "";
		Name = "";
		CreatedOn = DateTime.UtcNow;
	}

	public PlayerRecord(
		Id<Player> playerId,
		string name,
		DateTime createdOn
	) {
		PlayerId = playerId.Value;
		Name = name;
		CreatedOn = createdOn;
	}

	[DynamoDBHashKey( "PK" )]
	[DynamoDBRangeKey( "SK" )]
	public string PlayerId { get; set; }

	[DynamoDBProperty]
	public string Name { get; set; }

	[DynamoDBProperty]
	public DateTime CreatedOn { get; set; }

}
