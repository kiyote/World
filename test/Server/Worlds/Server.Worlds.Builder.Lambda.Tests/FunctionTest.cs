using Amazon.Lambda.SNSEvents;
using Amazon.Lambda.TestUtilities;

namespace Server.Worlds.Builder.Lambda.Tests;

[TestFixture]
public class FunctionTest {
	[Test]
	public async Task TestSQSEventLambdaFunction() {
		var snsEvent = new SNSEvent {
			Records = new List<SNSEvent.SNSRecord>
			{
				new SNSEvent.SNSRecord
				{
					Sns = new SNSEvent.SNSMessage()
					{
						Message = "foobar"
					}
				}
			}
		};

		var logger = new TestLambdaLogger();
		var context = new TestLambdaContext {
			Logger = logger
		};

		var function = new LambdaFunction();
		await function.FunctionHandler( snsEvent, context ).ConfigureAwait( false );

		Assert.That( $"Processed record foobar{Environment.NewLine}", Is.EqualTo( logger.Buffer.ToString() ) );
	}
}
