using Common.Worlds;
using Common.Worlds.Manager.Repositories;
using Server.Core.DynamoDb;
using Kiyote.Geometry;
using Point = Kiyote.Geometry.Point;

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
		ISize size = new Point( columns, rows );
		DateTime createdOn = new DateTime( 2022, 1, 21 );
		WorldRecord record = new WorldRecord( worldId, name, seed, rows, columns, createdOn );

		_db.Setup( db => db.CreateAsync( It.IsAny<WorldRecord>(), CancellationToken.None ) )
			.Callback( ( WorldRecord worldRecord, CancellationToken _ ) => {
				Assert.That( worldId.Value, Is.EqualTo( worldRecord.WorldId ) );
				Assert.That( name, Is.EqualTo( worldRecord.Name ) );
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

		Assert.That( worldId, Is.EqualTo( result.WorldId ) );
		Assert.That( name, Is.EqualTo( result.Name ) );
	}
}
