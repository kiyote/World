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
		services.TryAddSingleton<ISaltwaterBuilder, SaltwaterBuilder>();
		services.TryAddSingleton<IFreshwaterBuilder, FreshwaterBuilder>();
		services.TryAddSingleton<ILakeBuilder, LakeBuilder>();

		return services;
	}
}
