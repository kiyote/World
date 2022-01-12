namespace Updater.Actors;

using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using InjectableAWS;
using Microsoft.Extensions.DependencyInjection;
using Manager.Actors;
using Manager.Actors.Repositories.DynamoDb;

public class LambdaFunction {
	private readonly IServiceProvider _services;

	public LambdaFunction() {
		_services = ConfigureServices();
	}

	public async Task FunctionHandler(
		SQSEvent evnt,
		ILambdaContext context
	) {
		if( evnt is null
			|| context is null
		) {
			return;
		}

		using IServiceScope scope = _services.CreateScope();
		App app = scope.ServiceProvider.GetRequiredService<App>();

		foreach( SQSEvent.SQSMessage message in evnt.Records ) {
			await ProcessMessageAsync(
				message,
				context,
				app
			).ConfigureAwait( false );
		}
	}

	private static async Task ProcessMessageAsync(
		SQSEvent.SQSMessage message,
		ILambdaContext context,
		App app
	) {
		await app.ProcessMessageAsync(
			context.Logger
		).ConfigureAwait( false );
	}

	private static IServiceProvider ConfigureServices() {
		var services = new ServiceCollection();

		services.AddSingleton<App>();

		services.AddCredentials();
		services.AddActorManager();
		services.AddDynamoDbActors();

		return services.BuildServiceProvider();
	}
}

