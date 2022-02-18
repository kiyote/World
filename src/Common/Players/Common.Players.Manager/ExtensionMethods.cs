using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Common.Players.Manager;

public static class ExtensionMethods {
	public static IServiceCollection AddWorldManager(
		this IServiceCollection services
	) {

		services.TryAddSingleton<IPlayerManager, PlayerManager>();

		return services;
	}
}

