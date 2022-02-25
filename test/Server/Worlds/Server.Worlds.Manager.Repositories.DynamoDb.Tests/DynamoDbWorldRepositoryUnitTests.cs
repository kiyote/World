using Common.Worlds;
using Common.Worlds.Manager.Repositories;
using Moq;
using NUnit.Framework;
using Server.Core.DynamoDb;

namespace Server.Worlds.Manager.Repositories.DynamoDb.Tests;

public class DynamoDbWorldRepositoryUnitTests {

	private Mock<IWorldDynamoDbRepository> _db;
	private IWorldRepository _repository;

	[SetUp]
	public void SetUp() {
		_db = new Mock<IWorldDynamoDbRepository>( MockBehavior.Strict );
		_repository = new DynamoDbWorldRepository(
			_db.Object
		);
	}

	[Test]
	public async Task CreateAsync_ValidParameters_ActorCreated() {
		Id<World> worldId = new Id<World>( Guid.NewGuid() );
		string name = "world_name";
		string seed = "world_seed";
		int rows = 1;
		int columns = 2;
		Size size = new Size( columns, rows );
		DateTime createdOn = new DateTime( 2022, 1, 21 );
		WorldRecord record = new WorldRecord( worldId, name, seed, rows, columns, createdOn );

		_db.Setup( db => db.CreateAsync( It.IsAny<WorldRecord>(), CancellationToken.None ) )
			.Callback( ( WorldRecord worldRecord, CancellationToken _ ) => {
				Assert.AreEqual( worldId.Value, worldRecord.WorldId );
				Assert.AreEqual( name, worldRecord.Name );
			} )
			.Returns( Task.CompletedTask );

		World result = await _repository.CreateAsync(
			worldId,
			name,
			seed,
			size,
			createdOn,
			CancellationToken.None
		);

		Assert.AreEqual( worldId, result.WorldId );
		Assert.AreEqual( name, result.Name );
	}
}
