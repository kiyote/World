using Microsoft.Extensions.DependencyInjection;
using Common.Worlds.Builder.Noises;

namespace Common.Worlds.Builder;

public static class ExtensionMethods {

	public static IServiceCollection AddWorldBuilder(
		this IServiceCollection services
	) {
		services.AddSingleton<IWorldBuilder, WorldBuilder>();
		services.AddSingleton<INoiseProvider, OpenSimplexNoise>();
		services.AddSingleton<INoiseThresholder, SimpleNoiseThresholder>();
		services.AddSingleton<INoiseInverter, SimpleNoiseInverter>();

		return services;
	}
}
