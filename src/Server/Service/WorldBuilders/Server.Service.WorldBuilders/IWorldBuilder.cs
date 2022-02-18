using Common.Worlds;

namespace Server.Service.WorldBuilders;

internal interface IWorldBuilder {

	Task<Id<World>> BuildAsync(
		string name,
		string seed,
		int rows,
		int columns,
		CancellationToken cancellationToken
	);
}

