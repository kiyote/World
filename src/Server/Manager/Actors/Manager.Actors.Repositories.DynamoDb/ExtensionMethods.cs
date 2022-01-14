using InjectableAWS;
using Microsoft.Extensions.DependencyInjection;
using Common.DynamoDb;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Manager.Actors.Repositories.DynamoDb;

public static class ExtensionMethods {
	public static IServiceCollection AddDynamoDbActors(
		this IServiceCollection services
	) {
		services.AddDynamoDb<WorldDynamoDbRepositoryOptions>();
		services.TryAddSingleton<IActorRepository, DynamoDbActorRepository>();

		return services;
	}
}

