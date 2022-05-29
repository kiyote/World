using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Common.Worlds.Builder;

namespace Common.Worlds;

public static class ExtensionMethods {

	public static IServiceCollection AddWorlds(
		this IServiceCollection services
	) {
		services.TryAddSingleton<INeighbourLocator, HexNeighbourLocator>();
		services.TryAddSingleton<IWorldBuilder, WorldBuilder>();

		return services;
	}
}
