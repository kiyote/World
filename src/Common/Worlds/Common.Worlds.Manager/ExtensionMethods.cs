using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Common.Worlds.Manager;

public static class ExtensionMethods {
	public static IServiceCollection AddWorldManager(
		this IServiceCollection services
	) {

		services.TryAddSingleton<IWorldManager, WorldManager>();

		return services;
	}
}

