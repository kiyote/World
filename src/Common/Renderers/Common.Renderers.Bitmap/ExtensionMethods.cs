using Microsoft.Extensions.DependencyInjection;

namespace Common.Renderers.Bitmap;

public static class ExtensionMethods {

	public static IServiceCollection AddBitmapRenderer(
		this IServiceCollection services
	) {
		services.AddSingleton<IImageFactory, ImageFactory>();
		services.AddSingleton<IWorldRenderer, BitmapWorldRenderer>();

		return services;
	}
}
