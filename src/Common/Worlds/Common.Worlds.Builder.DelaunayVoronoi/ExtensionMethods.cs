using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Common.Worlds.Builder.DelaunayVoronoi;

public static class ExtensionMethods {

	public static IServiceCollection AddDelaunayVoronoiWorldBuilder(
		this IServiceCollection services
	) {
		services.AddRandomization();
		services.AddDelaunayVoronoi();

		services.TryAddSingleton<ILandformBuilder, TectonicLandformBuilder>();
		services.TryAddSingleton<IWorldMapGenerator, VoronoiWorldMapGenerator>();
		services.TryAddSingleton<IVoronoiBuilder, VoronoiBuilder>();
		services.TryAddSingleton<ISaltwaterFinder, MapEdgeSaltwaterFinder>();
		services.TryAddSingleton<IFreshwaterFinder, NotSaltwaterFreshwaterFinder>();
		services.TryAddSingleton<ILakeFinder, LandlockedLakeFinder>();
		services.TryAddSingleton<ICoastFinder, LandAdjacentCoastFinder>();

		services.TryAddSingleton<ITectonicPlateBuilder, TectonicPlateBuilder>();
		services.TryAddSingleton<IInlandDistanceFinder, LinearInlandDistanceFinder>();
		services.TryAddSingleton<IElevationBuilder, MountainousElevationBuilder>();
		services.TryAddSingleton<IElevationScaler, ExponentialElevationScaler>();

		return services;
	}
}
