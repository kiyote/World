using Common.Buffers;
using Common.Worlds.Manager;

namespace Common.Worlds.Builder;

internal sealed class WorldBuilder : IWorldBuilder {

	private readonly IWorldManager _worldManager;
	private readonly ITerrainMapGenerator _mapGenerator;

	public WorldBuilder(
		IWorldManager worldManager,
		ITerrainMapGenerator mapGenerator
	) {
		_worldManager = worldManager;
		_mapGenerator = mapGenerator;
	}

	Task<Id<World>> IWorldBuilder.BuildAsync(
		string name,
		string seed,
		Size size,
		CancellationToken cancellationToken
	) {
		Id<World> worldId = new Id<World>( Guid.NewGuid() );
		/*
		IBuffer<TileTerrain> terrain = _mapGenerator.GenerateTerrain( seed, size );

		//World world = new World( worldId, name, seed, rows, columns, DateTime.UtcNow );
		await _worldManager.CreateWorldAsync(
			worldId,
			name,
			worldId.ToString(),
			size,
			cancellationToken
		).ConfigureAwait( false );
		*/

		return Task.FromResult(worldId);
	}
}
