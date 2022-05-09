using Microsoft.Extensions.DependencyInjection;

namespace Common.Buffer.Unit;

public static class ExtensionMethods {

	public static IServiceCollection AddUnitBufferOperators(
		this IServiceCollection services
	) {
		services.AddSingleton<IUnitBufferLogicalOperators, UnitBufferLogicalOperators>();
		services.AddSingleton<IUnitBufferArithmeticOperators, UnitBufferArithmeticOperators>();
		services.AddSingleton<IUnitBufferClippingOperators, UnitBufferClippingOperators>();
		services.AddSingleton<IUnitBufferFilterOperators, UnitBufferFilterOperators>();
		services.AddSingleton<IUnitBufferNeighbourOperators, UnitBufferNeighbourOperators>();

		return services;
	}
}
