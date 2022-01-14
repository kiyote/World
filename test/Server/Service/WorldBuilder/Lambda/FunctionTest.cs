namespace Service.WorldBuilder.Lambda.Tests;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.SNSEvents;
using Amazon.Lambda.TestUtilities;
using NUnit.Framework;
using Service.WorldBuilder.Lambda;

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

		Assert.AreEqual( "Processed record foobar", logger.Buffer.ToString() );
	}
}
