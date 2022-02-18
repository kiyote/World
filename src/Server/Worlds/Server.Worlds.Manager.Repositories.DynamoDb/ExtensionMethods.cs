using Common.Worlds.Manager.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Server.Worlds.Manager.Repositories.DynamoDb;

public static class ExtensionMethods {

	public static IServiceCollection AddDynamoDbWorlds(
		this IServiceCollection services
	) {
		services.TryAddSingleton<IWorldRepository, DynamoDbWorldRepository>();

		return services;
	}
}
