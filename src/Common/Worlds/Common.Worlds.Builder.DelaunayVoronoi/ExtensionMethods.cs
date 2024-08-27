using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Common.Worlds.Builder.DelaunayVoronoi;

public static class ExtensionMethods {

	public static IServiceCollection AddDelaunayVoronoiWorldBuilder(
		this IServiceCollection services
	) {
		services.AddRandomization();
		services.AddDelaunayVoronoi();

		services.TryAddSingleton<ILandformBuilder, IterativeLandformBuilder>();
		services.TryAddSingleton<IWorldMapGenerator, VoronoiWorldMapGenerator>();
		services.TryAddSingleton<IVoronoiBuilder, VoronoiBuilder>();
		services.TryAddSingleton<IMountainsBuilder, MountainsBuilder>();
		services.TryAddSingleton<IHillsBuilder, HillsBuilder>();
		services.TryAddSingleton<ISaltwaterBuilder, SaltwaterBuilder>();
		services.TryAddSingleton<IFreshwaterBuilder, FreshwaterBuilder>();
		services.TryAddSingleton<IAirflowBuilder, AirflowBuilder>();
		services.TryAddSingleton<IMoistureBuilder, MoistureBuilder>();
		services.TryAddSingleton<ITemperatureBuilder, TemperatureBuilder>();
		services.TryAddSingleton<IForestBuilder, ForestBuilder>();
		services.TryAddSingleton<IDesertBuilder, DesertBuilder>();

		return services;
	}
}
