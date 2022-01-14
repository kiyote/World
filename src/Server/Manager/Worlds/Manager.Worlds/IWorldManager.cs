namespace Manager.Worlds;


public interface IWorldManager {
	Task<World> InitializeAsync(
		CancellationToken cancellationToken
	);
}
