using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Common.Buffers;

public static class ExtensionMethods {

	public static IServiceCollection AddCommonBuffers(
		this IServiceCollection services
	) {
		services.TryAddSingleton<IBufferOperator, BufferOperator>();
		services.TryAddSingleton<IBufferFactory, ArrayBufferFactory>();

		return services;
	}
}
