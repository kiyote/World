using Common.Worlds.Manager;

namespace Common.Worlds.Builder;

internal sealed class WorldBuilder : IWorldBuilder {

	private readonly IWorldManager _worldManager;
	private readonly IMapGenerator _mapGenerator;

	public WorldBuilder(
		IWorldManager worldManager,
		IMapGenerator mapGenerator
	) {
		_worldManager = worldManager;
		_mapGenerator = mapGenerator;
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

		TileTerrain[,] terrain = _mapGenerator.GenerateTerrain( seed, rows, columns );

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
