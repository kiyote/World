using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Common.Manager.Players;

public static class ExtensionMethods {
	public static IServiceCollection AddWorldManager(
		this IServiceCollection services
	) {

		services.TryAddSingleton<IPlayerManager, PlayerManager>();

		return services;
	}
}

