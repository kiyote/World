namespace Common.Worlds.Builder;

public interface IWorldBuilder {

	Task<Id<World>> BuildAsync(
		string name,
		string seed,
		int rows,
		int columns,
		CancellationToken cancellationToken
	);
}

