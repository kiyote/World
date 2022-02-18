using Common.Worlds.Manager;

namespace Common.Worlds.Builder;

internal sealed class WorldBuilder : IWorldBuilder {

	private readonly IWorldManager _worldManager;
	private readonly IRandom _random;

	public WorldBuilder(
		IWorldManager worldManager,
		IRandom random
	) {
		_worldManager = worldManager;
		_random = random;
	}

	async Task<Id<World>> IWorldBuilder.BuildAsync(
		string name,
		string seed,
		int rows,
		int columns,
		CancellationToken cancellationToken
	) {
		if (rows <= 0) {
			throw new ArgumentException( "Rows must be >= 0.", nameof( rows ) );
		}

		if (columns <= 0) {
			throw new ArgumentException( "Columns must be >= 0.", nameof( columns ) );
		}

		_random.Reinitialise( seed.GetHashCode( StringComparison.OrdinalIgnoreCase ) );

		Id<World> worldId = new Id<World>( Guid.NewGuid() );
		//World world = new World( worldId, name, seed, rows, columns, DateTime.UtcNow );
		await _worldManager.CreateWorldAsync(
			worldId,
			name,
			seed,
			rows,
			columns,
			cancellationToken
		).ConfigureAwait( false );

		return worldId;
	}
}
