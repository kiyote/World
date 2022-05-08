using Microsoft.Extensions.DependencyInjection;

namespace Common.Buffer.FloatingPoint;

public static class ExtensionMethods {

	public static IServiceCollection AddFloatingPointBufferOperators(
		this IServiceCollection services
	) {
		services.AddSingleton<IFloatBufferLogicalOperators, FloatBufferLogicalOperators>();
		services.AddSingleton<IFloatBufferArithmeticOperators, FloatBufferArithmeticOperators>();
		services.AddSingleton<IFloatBufferClippingOperators, FloatBufferClippingOperators>();
		services.AddSingleton<IFloatBufferFilterOperators, FloatBufferFilterOperators>();
		services.AddSingleton<IFloatBufferNeighbourOperators, FloatBufferNeighbourOperators>();

		return services;
	}
}
