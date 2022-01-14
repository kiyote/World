using InjectableAWS;
using Microsoft.Extensions.DependencyInjection;
using Common.DynamoDb;

namespace Manager.Actors.Repositories.DynamoDb;

public static class ExtensionMethods {
	public static IServiceCollection AddDynamoDbActors(
		this IServiceCollection services
	) {
		services.AddDynamoDb<WorldDynamoDbRepositoryOptions>();
		services.AddSingleton<IActorRepository, DynamoDbActorRepository>();

		return services;
	}
}

