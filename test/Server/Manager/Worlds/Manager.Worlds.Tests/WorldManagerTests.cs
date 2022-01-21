using NUnit.Framework;
using Moq;
using Manager.Worlds.Repositories;

namespace Manager.Worlds.Tests;

[TestFixture]
public class WorldManagerTests {
	private Mock<IWorldRepository> _worldRepository;
	private IWorldManager _worldManager;

	[SetUp]
	public void SetUp() {
		_worldRepository = new Mock<IWorldRepository>( MockBehavior.Strict );
		_worldManager = new WorldManager(
			_worldRepository.Object
		);
	}

	[Test]
	public async Task CreateAsync_ValidParameters_PlayerReturned() {
		Id<World> worldId = new Id<World>( Guid.NewGuid() );
		string name = "world_name";
		DateTime createdOn = new DateTime( 2022, 1, 13 );
		World world = new World( worldId, name, createdOn );
		_worldRepository
			.Setup( wr => wr.CreateAsync( worldId, name, It.IsAny<DateTime>(), CancellationToken.None ) )
			.Returns( Task.FromResult( world ) );

		World result = await _worldManager.CreateAsync( worldId, name, CancellationToken.None );

		Assert.AreEqual( worldId, result.Id );
		Assert.AreEqual( name, result.Name );
	}
}
