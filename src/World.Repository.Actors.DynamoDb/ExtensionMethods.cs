namespace World.Repository.Actors.DynamoDb;

using InjectableAWS;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class ExtensionMethods {
	public static IServiceCollection AddDynamoDbActors(
		this IServiceCollection services,
		IConfigurationSection configuration
	) {
		services.AddDynamoDb<DynamoDbActorRepositoryConfiguration>( configuration );
		services.AddSingleton<IActorRepository, DynamoDbActorRepository>();

		return services;
	}
}

