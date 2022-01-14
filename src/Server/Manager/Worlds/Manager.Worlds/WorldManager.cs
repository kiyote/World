namespace Manager.Worlds;

[System.Diagnostics.CodeAnalysis.SuppressMessage( "Performance", "CA1812:An internal (assembly-level) type is never instantiated.", Justification = "This class is instantiated via DI." )]
internal class WorldManager : IWorldManager {
	Task<World> IWorldManager.InitializeAsync( CancellationToken cancellationToken ) {
		return Task.FromResult( new World( new Id<World>(Guid.NewGuid()), "Name", DateTime.UtcNow ) );
	}
}

