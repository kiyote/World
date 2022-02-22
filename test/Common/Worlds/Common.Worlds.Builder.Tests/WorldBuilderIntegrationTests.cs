using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Common.Worlds.Builder.Tests;

[TestFixture]
public sealed class WorldBuilderIntegrationTests {

	private IWorldBuilder _worldBuilder;
	private IServiceProvider _provider;
	private IServiceScope _scope;
	private string _folder;

	[OneTimeSetUp]
	public void OneTimeSetUp() {
		_folder = Path.Combine( Path.GetTempPath(), Guid.NewGuid().ToString( "N" ) );
		var services = new ServiceCollection();
		services.AddWorldBuilder();

		_provider = services.BuildServiceProvider();
	}

	[OneTimeTearDown]
	public void OneTimeTearDown() {
		Directory.Delete( _folder, true );
	}

	[SetUp]
	public void SetUp() {
		_scope = _provider.CreateScope();

		_worldBuilder = _provider.GetRequiredService<IWorldBuilder>();
	}

	[TearDown]
	public void TearDown() {
		_scope.Dispose();
	}
}

