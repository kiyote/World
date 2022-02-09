using NUnit.Framework;
using Moq;
using Common.Manager.Actors.Repositories;

namespace Common.Manager.Actors.Tests;

[TestFixture]
public class ActorManagerTests {
	private Mock<IActorRepository> _actorRepository;
	private IActorManager _actorManager;

	[SetUp]
	public void SetUp() {
		_actorRepository = new Mock<IActorRepository>( MockBehavior.Strict );
		_actorManager = new ActorManager(
			_actorRepository.Object
		);
	}

	[Test]
	public async Task CreateAsync_ValidParameters_PlayerReturned() {
		Id<Actor> actorId = new Id<Actor>( Guid.NewGuid() );
		Id<World> worldId = new Id<World>( Guid.NewGuid() );
		string name = "actor_name";
		DateTime createdOn = new DateTime( 2022, 1, 13 );
		Actor actor = new Actor( actorId, name, createdOn );
		_actorRepository
			.Setup( ar => ar.CreateAsync( worldId, actorId, name, It.IsAny<DateTime>(), CancellationToken.None ) )
			.Returns( Task.FromResult( actor ) );


		Actor result = await _actorManager.CreateAsync( worldId, actorId, name, CancellationToken.None );

		Assert.AreEqual( actorId, result.ActorId );
		Assert.AreEqual( name, result.Name );
	}
}
