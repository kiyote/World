namespace World.Updater.Actors;

using Amazon.Lambda.Core;

#pragma warning disable CA1812
internal class App {
#pragma warning restore CA1812

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
