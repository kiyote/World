﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Common.Geometry.DelaunayVoronoi;

namespace Common.Geometry;

public static class ExtensionMethods {

	public static IServiceCollection AddGeometry(
		this IServiceCollection services
	) {
		services.TryAddSingleton<IGeometry, Geometry>();
		services.TryAddSingleton<IDelaunatorFactory, DelaunatorFactory>();
		services.TryAddSingleton<IDelaunayFactory, DelaunayFactory>();
		services.TryAddSingleton<IVoronoiFactory, VoronoiFactory>();

		return services;
	}
}