using Microsoft.Extensions.DependencyInjection;

namespace Service.WorldBuilder;

public static class ExtensionMethods {

	public static IServiceCollection AddWorldBuilder(
		this IServiceCollection services
	) {
		services.AddSingleton<IWorldBuilder, DefaultWorldBuilder>();

		return services;
	}
}
