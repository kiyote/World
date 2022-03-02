using Microsoft.Extensions.DependencyInjection;

namespace Common.Renderers.Minimap;

public static class ExtensionMethods {

	public static IServiceCollection AddMinimapRenderer(
		this IServiceCollection services
	) {
		services.AddSingleton<IImageFactory, ImageFactory>();
		services.AddSingleton<IWorldRenderer, MinimapWorldRenderer>();

		return services;
	}
}
