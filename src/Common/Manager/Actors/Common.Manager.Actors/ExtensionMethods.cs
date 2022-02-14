﻿using Microsoft.Extensions.DependencyInjection;

namespace Common.Manager.Actors;

public static class ExtensionMethods {

	public static IServiceCollection AddActorManager(
		this IServiceCollection services
	) {

		services.AddSingleton<IActorManager, ActorManager>();

		return services;
	}
}