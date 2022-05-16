using Common.Geometry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Common.Worlds.Builder;

public static class ExtensionMethods {

	public static IServiceCollection AddWorldBuilder(
		this IServiceCollection services
	) {
		services.AddGeometry();
		services.TryAddSingleton<INeighbourLocator, HexNeighbourLocator>();
		services.TryAddSingleton<IWorldBuilder, WorldBuilder>();
		services.TryAddSingleton<ILandformMapGenerator, VoronoiLandformMapGenerator>();
		services.TryAddSingleton<ITemperatureMapGenerator, TemperatureMapGenerator>();
		services.TryAddSingleton<IAirFlowMapGenerator, AirFlowMapGenerator>();
		services.TryAddSingleton<IVoronoiBuilder, VoronoiBuilder>();
		services.TryAddSingleton<IRoughLandformBuilder, RoughLandformBuilder>();
		services.TryAddSingleton<IFineLandformBuilder, FineLandformBuilder>();
		services.TryAddSingleton<IMountainRangeBuilder, MountainRangeBuilder>();

		return services;
	}
}
