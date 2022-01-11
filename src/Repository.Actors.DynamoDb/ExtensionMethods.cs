namespace Repository.Actors.DynamoDb;

using InjectableAWS;
using Microsoft.Extensions.DependencyInjection;
using Repository.DynamoDb;

public static class ExtensionMethods {
	public static IServiceCollection AddDynamoDbActors(
		this IServiceCollection services
	) {
		services.AddDynamoDb<WorldDynamoDbRepositoryOptions>();
		services.AddSingleton<IActorRepository, DynamoDbActorRepository>();

		return services;
	}
}

