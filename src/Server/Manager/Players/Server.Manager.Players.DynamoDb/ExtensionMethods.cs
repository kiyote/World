using Common.Manager.Players.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Server.Manager.Players.Repositories.DynamoDb;

public static class ExtensionMethods {

	public static IServiceCollection AddDynamoDbPlayers(
		this IServiceCollection services
	) {
		services.TryAddSingleton<IPlayerRepository, DynamoDbPlayerRepository>();

		return services;
	}
}
