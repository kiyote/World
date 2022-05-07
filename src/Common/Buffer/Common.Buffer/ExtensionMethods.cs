using Microsoft.Extensions.DependencyInjection;

namespace Common.Buffer;

public static class ExtensionMethods {

	public static IServiceCollection AddArrayBuffer(
		this IServiceCollection services
	) {
		services.AddSingleton<IBufferOperator, DelegateBufferOperator>();
		services.AddSingleton<IBufferFactory, ArrayBufferFactory>();

		return services;
	}
}
