namespace Common.Worlds.Builder.Tests;

[TestFixture]
public sealed class WorldBuilderIntegrationTests {

	private IWorldBuilder _worldBuilder;
	private IServiceProvider _provider;
	private IServiceScope _scope;
	private string _folder;

	[OneTimeSetUp]
	public void OneTimeSetUp() {
		string rootPath = Path.Combine( Path.GetTempPath(), "world" );
		Directory.CreateDirectory( rootPath );
		_folder = Path.Combine( rootPath, nameof( WorldBuilderIntegrationTests ) );
		var services = new ServiceCollection();
		services.AddCoreWorldBuilder();

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

