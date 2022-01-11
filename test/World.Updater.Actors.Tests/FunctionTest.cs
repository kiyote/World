namespace World.Updater.Actors.Tests;

using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.SQSEvents;
using Amazon.Lambda.TestUtilities;
using NUnit.Framework;

public class FunctionTest {
	[Test]
	public async Task TestSQSEventLambdaFunction() {
		var sqsEvent = new SQSEvent {
			Records = new List<SQSEvent.SQSMessage>
			{
				new SQSEvent.SQSMessage
				{
					Body = "foobar"
				}
			}
		};

		var logger = new TestLambdaLogger();
		var context = new TestLambdaContext {
			Logger = logger
		};

		var function = new LambdaFunction();
		await function.FunctionHandler( sqsEvent, context ).ConfigureAwait( false );

		Assert.AreEqual( "Test\r\n", logger.Buffer.ToString() );
	}
}
