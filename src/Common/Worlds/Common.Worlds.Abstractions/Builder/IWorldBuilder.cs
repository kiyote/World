namespace Common.Worlds.Builder;

public interface IWorldBuilder {

	Task<Id<World>> BuildAsync(
		string name,
		string seed,
		ISize size,
		CancellationToken cancellationToken
	);
}

