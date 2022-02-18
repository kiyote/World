using Common.Actors.Manager.Repositories;
using InjectableAWS;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Server.Core.DynamoDb;

namespace Server.Actors.Manager.Repositories.DynamoDb;

public static class ExtensionMethods {
	public static IServiceCollection AddDynamoDbActors(
		this IServiceCollection services
	) {
		services.AddDynamoDb<WorldDynamoDbRepositoryOptions>();
		services.TryAddSingleton<IActorRepository, DynamoDbActorRepository>();

		return services;
	}
}

