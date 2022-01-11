using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Microsoft.Extensions.DependencyInjection;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer( typeof( Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer ) )]

namespace ActorUpdater {
	public class Function {
		private readonly IServiceProvider _services;

		/// <summary>
		/// Default constructor. This constructor is used by Lambda to construct the instance. When invoked in a Lambda environment
		/// the AWS credentials will come from the IAM role associated with the function and the AWS region will be set to the
		/// region the Lambda function is executed in.
		/// </summary>
		public Function() {
			_services = ConfigureServices();
		}


		/// <summary>
		/// This method is called for every Lambda invocation. This method takes in an SQS event object and can be used 
		/// to respond to SQS messages.
		/// </summary>
		/// <param name="evnt"></param>
		/// <param name="context"></param>
		/// <returns></returns>
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

		private async Task ProcessMessageAsync(
			SQSEvent.SQSMessage message,
			ILambdaContext context,
			App app
		) {
			context.Logger.LogLine( $"Processed message {message.Body}" );

			// TODO: Do interesting work based on the new message
			await Task.CompletedTask;
		}

		private static IServiceProvider ConfigureServices() {
			var services = new ServiceCollection();

			services.AddSingleton<App>();

			return services.BuildServiceProvider();
		}
	}
}
