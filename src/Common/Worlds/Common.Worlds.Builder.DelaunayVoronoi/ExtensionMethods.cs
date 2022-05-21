using Common.Geometry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Common.Worlds.Builder.DelaunayVoronoi;

public static class ExtensionMethods {

	public static IServiceCollection AddDelaunayVoronoiWorldBuilder(
		this IServiceCollection services
	) {
		services.AddGeometry();
		services.AddWorldBuilder();
		services.TryAddSingleton<ILandformMapGenerator, LandformMapGenerator>();
		services.TryAddSingleton<IVoronoiBuilder, VoronoiBuilder>();
		services.TryAddSingleton<ILandformBuilder, LandformBuilder>();
		services.TryAddSingleton<IMountainsBuilder, MountainsBuilder>();
		services.TryAddSingleton<IHillsBuilder, HillsBuilder>();
		services.TryAddSingleton<ISaltwaterBuilder, SaltwaterBuilder>();
		services.TryAddSingleton<IFreshwaterBuilder, FreshwaterBuilder>();

		return services;
	}
}
