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
		Size size,
		CancellationToken cancellationToken
	) {
		TileTerrain[,] terrain = _mapGenerator.GenerateTerrain( seed, size );

		Id<World> worldId = new Id<World>( Guid.NewGuid() );
		//World world = new World( worldId, name, seed, rows, columns, DateTime.UtcNow );
		await _worldManager.CreateWorldAsync(
			worldId,
			name,
			seed,
			size,
			cancellationToken
		).ConfigureAwait( false );

		return worldId;
	}
}
