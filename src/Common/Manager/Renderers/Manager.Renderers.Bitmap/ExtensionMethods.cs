namespace Service.Renderers.Bitmap;
using Microsoft.Extensions.DependencyInjection;

public static class ExtensionMethods {

	public static IServiceCollection AddBitmapRenderer(
		this IServiceCollection services
	) {
		services.AddSingleton<IWorldRenderer, BitmapWorldRenderer>();

		return services;
	}
}
