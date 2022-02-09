using InjectableAWS;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Server.Common.DynamoDb;
using Common.Manager.Actors.Repositories;

namespace Server.Manager.Actors.Repositories.DynamoDb;

public static class ExtensionMethods {
	public static IServiceCollection AddDynamoDbActors(
		this IServiceCollection services
	) {
		services.AddDynamoDb<WorldDynamoDbRepositoryOptions>();
		services.TryAddSingleton<IActorRepository, DynamoDbActorRepository>();

		return services;
	}
}

