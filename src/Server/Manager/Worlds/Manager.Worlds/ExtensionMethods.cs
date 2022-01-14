using Microsoft.Extensions.DependencyInjection;

namespace Manager.Worlds;

public static class ExtensionMethods {
	public static IServiceCollection AddWorldManager(
		this IServiceCollection services
	) {

		services.AddSingleton<IWorldManager, WorldManager>();

		return services;
	}
}

