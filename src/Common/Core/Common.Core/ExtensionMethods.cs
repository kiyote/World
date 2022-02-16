using Microsoft.Extensions.DependencyInjection;

namespace Common.Core;

public static class ExtensionMethods {

	public static IServiceCollection AddCore(
		this IServiceCollection services
	) {
	
		services.AddSingleton<IRandom, FastRandom>();

		return services;
	}
}

