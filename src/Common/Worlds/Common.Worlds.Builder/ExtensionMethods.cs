using Microsoft.Extensions.DependencyInjection;
using Common.Worlds.Builder.Noises;
using Common.Worlds.Builder.DelaunayVoronoi;

namespace Common.Worlds.Builder;

public static class ExtensionMethods {

	public static IServiceCollection AddWorldBuilder(
		this IServiceCollection services
	) {
		services.AddSingleton<INeighbourLocator, HexNeighbourLocator>();
		//services.AddSingleton<IMapGenerator, MapGenerator>();
		services.AddSingleton<IWorldBuilder, WorldBuilder>();
		services.AddSingleton<INoiseProvider, OpenSimplexNoise>();
		services.AddSingleton<INoiseMaskGenerator, SimpleNoiseMaskGenerator>();
		services.AddSingleton<ILandformGenerator, IslandLandformGenerator>();
		services.AddSingleton<IDelaunatorFactory, DelaunatorFactory>();
		services.AddSingleton<IVoronoiFactory, VoronoiFactory>();

		return services;
	}
}
