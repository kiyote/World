using Common.Worlds.Manager;

namespace Common.Worlds.Builder;

internal sealed class WorldBuilder : IWorldBuilder {

	private readonly ILandformMapGenerator _landformMapGenerator;
	private readonly IWorldManager _worldManager;
	private readonly INeighbourLocator _neighbourLocator;

	public WorldBuilder(
		IWorldManager worldManager,
		ILandformMapGenerator landformMapGenerator,
		INeighbourLocator neighbourLocator
	) {
		_worldManager = worldManager;
		_landformMapGenerator = landformMapGenerator;
		_neighbourLocator = neighbourLocator;

	}

	Task<Id<World>> IWorldBuilder.BuildAsync(
		string name,
		string seed,
		Size size,
		CancellationToken cancellationToken
	) {
		Id<World> worldId = new Id<World>( Guid.NewGuid() );

		LandformMaps maps = _landformMapGenerator.Create(
			Hash.GetLong( seed ),
			size,
			_neighbourLocator
		);

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

		return Task.FromResult( worldId );
	}
}
