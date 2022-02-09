namespace Common.Service.Worlds;

public interface IWorldsService {
	World GetWorldById( Id<World> worldId );
}
