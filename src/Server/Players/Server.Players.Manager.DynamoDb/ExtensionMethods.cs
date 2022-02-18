using Common.Players.Manager.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Server.Players.Manager.Repositories.DynamoDb;

public static class ExtensionMethods {

	public static IServiceCollection AddDynamoDbPlayers(
		this IServiceCollection services
	) {
		services.TryAddSingleton<IPlayerRepository, DynamoDbPlayerRepository>();

		return services;
	}
}
