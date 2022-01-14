using Microsoft.Extensions.DependencyInjection;

namespace Service.WorldBuilders;

public static class ExtensionMethods {

	public static IServiceCollection AddWorldBuilder(
		this IServiceCollection services
	) {
		services.AddSingleton<IWorldBuilder, DefaultWorldBuilder>();

		return services;
	}
}
