using Common.Worlds.Manager.Repositories;
using Moq;
using NUnit.Framework;

namespace Common.Worlds.Manager.Tests;

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
		int rows = 1;
		int columns = 2;
		DateTime createdOn = new DateTime( 2022, 1, 13 );
		World world = new World( worldId, name, seed, rows, columns, createdOn );
		_worldRepository
			.Setup( wr => wr.CreateAsync( worldId, name, seed, rows, columns, It.IsAny<DateTime>(), CancellationToken.None ) )
			.Returns( Task.FromResult( world ) );

		World result = await _worldManager.CreateWorldAsync( worldId, name, seed, rows, columns, CancellationToken.None );

		Assert.AreEqual( worldId, result.WorldId );
		Assert.AreEqual( name, result.Name );
	}
}
