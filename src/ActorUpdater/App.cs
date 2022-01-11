namespace ActorUpdater;

using Amazon.Lambda.Core;

internal class App {
	private readonly string _message;

	public App() {
		_message = "Test";
	}

	public Task ProcessMessageAsync(
		ILambdaLogger logger
	) {
		logger.LogLine( _message );
		return Task.CompletedTask;
	}
}

