using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Common.Core;

public static class ExtensionMethods {

	public static IServiceCollection AddCommonCore(
		this IServiceCollection services
	) {
	
		services.TryAddSingleton<IRandom, FastRandom>();

		return services;
	}
}

