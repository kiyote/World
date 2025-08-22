using Common.Worlds.Manager;

namespace Common.Worlds.Builder;

internal sealed class WorldBuilder : IWorldBuilder {

	private readonly IWorldMapGenerator _worldMapGenerator;
	private readonly IWorldManager _worldManager;
	private readonly INeighbourLocator _neighbourLocator;

	public WorldBuilder(
		IWorldManager worldManager,
		IWorldMapGenerator landformMapGenerator,
		INeighbourLocator neighbourLocator
	) {
		_worldManager = worldManager;
		_worldMapGenerator = landformMapGenerator;
		_neighbourLocator = neighbourLocator;

	}

	async Task<Id<World>> IWorldBuilder.BuildAsync(
		string name,
		string seed,
		ISize size,
		CancellationToken cancellationToken
	) {
		Id<World> worldId = new Id<World>( Guid.NewGuid() );

		WorldMaps maps = await _worldMapGenerator.CreateAsync(
			Hash.GetLong( seed ),
			size,
			_neighbourLocator,
			cancellationToken
		).ConfigureAwait( false );

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

		return worldId;
	}
}
