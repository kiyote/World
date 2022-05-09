using Microsoft.Extensions.DependencyInjection;

namespace Common.Buffer.Float;

public static class ExtensionMethods {

	public static IServiceCollection AddFloatBufferOperators(
		this IServiceCollection services
	) {
		services.AddSingleton<IFloatBufferOperators, FloatBufferOperators>();
		return services;
	}

}
