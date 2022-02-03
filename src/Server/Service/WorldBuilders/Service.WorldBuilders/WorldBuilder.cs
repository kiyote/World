namespace Service.WorldBuilders;

public class WorldBuilder : IWorldBuilder {

	private readonly IWorldManager _worldManager;
	private readonly IRandom _random;

	public WorldBuilder(
		IWorldManager worldManager,
		IRandom random
	) {
		_worldManager = worldManager;
		_random = random;
	}

	void IWorldBuilder.Build(
		string seed
	) {
		_random.Reinitialise( seed.GetHashCode( StringComparison.Ordinal ) );
	}
}
