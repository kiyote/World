using Microsoft.Extensions.DependencyInjection;

namespace Common.Buffer;

public static class ExtensionMethods {

	public static IServiceCollection AddArrayBufferFactory(
		this IServiceCollection services
	) {
		services.AddSingleton<IBufferFactory, ArrayBufferFactory>();

		return services;
	}
}
