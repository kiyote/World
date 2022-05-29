using Amazon.Lambda.Core;
using Amazon.Lambda.SNSEvents;
using Microsoft.Extensions.DependencyInjection;

namespace Server.Worlds.Builder.Lambda;

public class LambdaFunction {
	private readonly IServiceProvider _services;

	public LambdaFunction() {
		_services = ConfigureServices();
	}

	public async Task FunctionHandler(
		SNSEvent evnt,
		ILambdaContext context
	) {
		if (evnt is null
			|| context is null
		) {
			return;
		}

		using IServiceScope scope = _services.CreateScope();
		foreach( SNSEvent.SNSRecord? record in evnt.Records ) {
			await ProcessRecordAsync( record, context ).ConfigureAwait( false );
		}
	}

	private static Task ProcessRecordAsync(
		SNSEvent.SNSRecord record,
		ILambdaContext context
	) {
		context.Logger.LogLine( $"Processed record {record.Sns.Message}" );

		// TODO: Do interesting work based on the new message
		return Task.CompletedTask;
	}

	private static IServiceProvider ConfigureServices() {
		var services = new ServiceCollection();

		services.AddCore();
		services.AddWorlds();

		return services.BuildServiceProvider();
	}
}
