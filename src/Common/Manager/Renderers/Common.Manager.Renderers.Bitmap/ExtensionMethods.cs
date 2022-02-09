using Microsoft.Extensions.DependencyInjection;

namespace Common.Service.Renderers.Bitmap;

public static class ExtensionMethods {

	public static IServiceCollection AddBitmapRenderer(
		this IServiceCollection services
	) {
		services.AddSingleton<IWorldRenderer, BitmapWorldRenderer>();

		return services;
	}
}
