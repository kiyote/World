namespace Service.WorldBuilders;

public class WorldBuilder: IWorldBuilder {

	private readonly IWorldManager _worldManager;

	public WorldBuilder(
		IWorldManager worldManager
	) {
		_worldManager = worldManager;
	}

}
