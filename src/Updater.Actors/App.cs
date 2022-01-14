using Amazon.Lambda.Core;

namespace Updater.Actors;

[System.Diagnostics.CodeAnalysis.SuppressMessage( "Performance", "CA1812:An internal (assembly-level) type is never instantiated.", Justification = "This class is instantiated via DI." )]
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
