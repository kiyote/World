using Common.Geometry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Common.Worlds.Builder;

public static class ExtensionMethods {

	public static IServiceCollection AddCoreWorldBuilder(
		this IServiceCollection services
	) {
		services.AddGeometry();
		services.TryAddSingleton<INeighbourLocator, HexNeighbourLocator>();
		services.TryAddSingleton<IWorldBuilder, WorldBuilder>();

		return services;
	}
}
