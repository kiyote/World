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
		services.TryAddSingleton<IRoughLandformBuilder, RoughLandformBuilder>();
		services.TryAddSingleton<IFineLandformBuilder, FineLandformBuilder>();
		services.TryAddSingleton<IMountainRangeBuilder, MountainRangeBuilder>();

		return services;
	}
}
