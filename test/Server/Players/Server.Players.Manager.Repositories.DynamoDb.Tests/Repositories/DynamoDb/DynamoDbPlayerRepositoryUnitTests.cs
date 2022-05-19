using Common.Players;
using Common.Players.Manager.Repositories;
using Server.Core.DynamoDb;

namespace Server.Players.Manager.Repositories.DynamoDb.Tests;

public class DynamoDbPlayerRepositoryUnitTests {

	private Mock<IWorldDynamoDbRepository> _db;
	private IPlayerRepository _repository;

	[SetUp]
	public void SetUp() {
		_db = new Mock<IWorldDynamoDbRepository>( MockBehavior.Strict );
		_repository = new DynamoDbPlayerRepository(
			_db.Object
		);
	}

	[Test]
	public async Task CreateAsync_ValidParameters_ActorCreated() {
		Id<Player> playerId = new Id<Player>( Guid.NewGuid() );
		string name = "player_name";
		DateTime createdOn = new DateTime( 2022, 1, 21 );
		PlayerRecord record = new PlayerRecord( playerId, name, createdOn );

		_db.Setup( db => db.CreateAsync( It.IsAny<PlayerRecord>(), CancellationToken.None ) )
			.Callback( ( PlayerRecord playerRecord, CancellationToken _ ) => {
				Assert.AreEqual( playerId.Value, playerRecord.PlayerId );
				Assert.AreEqual( name, playerRecord.Name );
			} )
			.Returns( Task.CompletedTask );

		Player result = await _repository.CreateAsync(
			playerId,
			name,
			createdOn,
			CancellationToken.None
		);

		Assert.AreEqual( playerId, result.PlayerId );
		Assert.AreEqual( name, result.Name );
	}
}
