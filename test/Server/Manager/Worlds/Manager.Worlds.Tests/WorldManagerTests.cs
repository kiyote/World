using NUnit.Framework;
using Moq;
using Manager.Worlds.Repositories;

namespace Manager.Worlds.Tests;

[TestFixture]
public class WorldManagerTests {
	private Mock<IWorldRepository> _worldRepository;
	private Mock<IRegionRepository> _regionRepository;
	private IWorldManager _worldManager;

	[SetUp]
	public void SetUp() {
		_worldRepository = new Mock<IWorldRepository>( MockBehavior.Strict );
		_regionRepository = new Mock<IRegionRepository>( MockBehavior.Strict );
		_worldManager = new WorldManager(
			_worldRepository.Object,
			_regionRepository.Object
		);
	}

	[Test]
	public async Task CreateWorldAsync_ValidParameters_PlayerReturned() {
		Id<World> worldId = new Id<World>( Guid.NewGuid() );
		string name = "world_name";
		string seed = "world_seed";
		DateTime createdOn = new DateTime( 2022, 1, 13 );
		World world = new World( worldId, name, seed, createdOn );
		_worldRepository
			.Setup( wr => wr.CreateAsync( worldId, name, seed, It.IsAny<DateTime>(), CancellationToken.None ) )
			.Returns( Task.FromResult( world ) );

		World result = await _worldManager.CreateWorldAsync( worldId, name, seed, CancellationToken.None );

		Assert.AreEqual( worldId, result.WorldId );
		Assert.AreEqual( name, result.Name );
	}
}
