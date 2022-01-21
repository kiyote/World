using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Manager.Players.Repositories.DynamoDb;

public static class ExtensionMethods {

	public static IServiceCollection AddDynamoDbPlayers(
		this IServiceCollection services
	) {
		services.TryAddSingleton<IPlayerRepository, DynamoDbPlayerRepository>();

		return services;
	}
}
