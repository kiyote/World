using Common.Players.Manager.Repositories;
using Moq;
using NUnit.Framework;

namespace Common.Players.Manager.Tests;

[TestFixture]
public class PlayerManagerUnitTests {
	private Mock<IPlayerRepository> _playerRepository;
	private IPlayerManager _playerManager;

	[SetUp]
	public void SetUp() {
		_playerRepository = new Mock<IPlayerRepository>( MockBehavior.Strict );
		_playerManager = new PlayerManager(
			_playerRepository.Object
		);
	}

	[Test]
	public async Task CreateAsync_ValidParameters_PlayerReturned() {
		Id<Player> playerId = new Id<Player>( Guid.NewGuid() );
		string name = "player_name";
		DateTime createdOn = new DateTime( 2022, 1, 13 );
		Player player = new Player( playerId, name, createdOn );
		_playerRepository
			.Setup( pr => pr.CreateAsync( playerId, name, It.IsAny<DateTime>(), CancellationToken.None ) )
			.Returns( Task.FromResult( player ) );


		Player result = await _playerManager.CreateAsync( playerId, name, CancellationToken.None );

		Assert.AreEqual( playerId, result.PlayerId );
		Assert.AreEqual( name, result.Name );
	}
}
