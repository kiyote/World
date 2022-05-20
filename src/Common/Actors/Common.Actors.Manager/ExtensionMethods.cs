using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Common.Actors.Manager;

public static class ExtensionMethods {

	public static IServiceCollection AddActors(
		this IServiceCollection services
	) {

		services.TryAddSingleton<IActorManager, ActorManager>();

		return services;
	}
}
