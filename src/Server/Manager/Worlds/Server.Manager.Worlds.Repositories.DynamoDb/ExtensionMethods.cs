using Common.Manager.Worlds.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Server.Manager.Worlds.Repositories.DynamoDb;

public static class ExtensionMethods {

	public static IServiceCollection AddDynamoDbWorlds(
		this IServiceCollection services
	) {
		services.TryAddSingleton<IWorldRepository, DynamoDbWorldRepository>();

		return services;
	}
}
