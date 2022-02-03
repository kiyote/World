using Microsoft.Extensions.DependencyInjection;

namespace Core;

public static class ExtensionMethods {

	public static IServiceCollection AddCore(
		this IServiceCollection services
	) {
	
		services.AddSingleton<IRandom, FastRandom>();

		return services;
	}
}

