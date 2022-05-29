using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Common.Buffers.Float;

public static class ExtensionMethods {

	public static IServiceCollection AddCommonBuffersFloat(
		this IServiceCollection services
	) {
		services.TryAddSingleton<IFloatBufferOperators, FloatBufferOperators>();
		return services;
	}

}
