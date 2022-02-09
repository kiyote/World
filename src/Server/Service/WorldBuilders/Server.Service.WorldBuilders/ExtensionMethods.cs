using Microsoft.Extensions.DependencyInjection;

namespace Server.Service.WorldBuilders;

public static class ExtensionMethods {

	public static IServiceCollection AddWorldBuilder(
		this IServiceCollection services
	) {
		services.AddSingleton<IWorldBuilder, WorldBuilder>();

		return services;
	}
}
