using NUnit.Framework;
using Moq;
using Common.DynamoDb;

namespace Manager.Actors.Repositories.DynamoDb.Tests;

public class DynamoDbActorRepositoryTests {

	private Mock<IWorldDynamoDbRepository> _db;
	private IActorRepository _repository;

	[SetUp]
	public void SetUp() {
		_db = new Mock<IWorldDynamoDbRepository>( MockBehavior.Strict );
		_repository = new DynamoDbActorRepository(
			_db.Object
		);
	}

	[Test]
	public async Task CreateAsync_ValidParameters_ActorCreated() {
		Id<World> worldId = new Id<World>( Guid.NewGuid() );
		Id<Actor> actorId = new Id<Actor>( Guid.NewGuid() );
		string name = "actor_name";
		DateTime createdOn = new DateTime( 2022, 1, 21 );
		ActorRecord record = new ActorRecord( worldId, actorId, name, createdOn );

		_db.Setup( db => db.CreateAsync( It.IsAny<ActorRecord>(), CancellationToken.None ) )
			.Callback( (ActorRecord actorRecord, CancellationToken _) => {
				Assert.AreEqual( worldId.Value, actorRecord.WorldId );
				Assert.AreEqual( actorId.Value, actorRecord.ActorId );
				Assert.AreEqual( name, actorRecord.Name );
			} )
			.Returns( Task.CompletedTask );

		Actor result = await _repository.CreateAsync(
			worldId,
			actorId,
			name,
			createdOn,
			CancellationToken.None
		);

		Assert.AreEqual( actorId, result.ActorId );
		Assert.AreEqual( name, result.Name );
	}
}
