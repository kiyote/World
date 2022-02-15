using Microsoft.Extensions.DependencyInjection;
using Server.Service.WorldBuilders.Noises;

namespace Server.Service.WorldBuilders;

public static class ExtensionMethods {

	public static IServiceCollection AddWorldBuilder(
		this IServiceCollection services
	) {
		services.AddSingleton<IWorldBuilder, WorldBuilder>();
		services.AddSingleton<INoiseProvider, OpenSimplexNoise>();

		return services;
	}
}
