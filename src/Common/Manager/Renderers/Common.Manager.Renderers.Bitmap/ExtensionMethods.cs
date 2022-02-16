﻿using Microsoft.Extensions.DependencyInjection;

namespace Common.Manager.Renderers.Bitmap;

public static class ExtensionMethods {

	public static IServiceCollection AddBitmapRenderer(
		this IServiceCollection services
	) {
		services.AddSingleton<IWorldRenderer, BitmapWorldRenderer>();

		return services;
	}
}
