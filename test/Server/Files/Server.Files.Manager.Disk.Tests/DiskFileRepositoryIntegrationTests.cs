using Common.Files;
using Common.Files.Manager.Repositories;

namespace Server.Files.Manager.Disk.Tests;

[TestFixture]
public class DiskFileRepositoryIntegrationTests {

	private IServiceScope _scope;
	private IServiceProvider _provider;
	private IDiskFileRepository _diskRepository;
	private string _testFolder;

	[OneTimeSetUp]
	public void OneTimeSetUp() {
		string root = Path.Combine( Path.GetTempPath(), "world" );
		_testFolder = Path.Combine( root, nameof( DiskFileRepositoryIntegrationTests ) );
		Directory.CreateDirectory( _testFolder );

		var services = new ServiceCollection();
		services.AddDiskFileManager( _testFolder );

		_provider = services.BuildServiceProvider();
		_diskRepository = _provider.GetRequiredService<IDiskFileRepository>();
	}

	[SetUp]
	public void SetUp() {
		_scope = _provider.CreateScope();
	}

	[TearDown]
	public void TearDown() {
		_scope.Dispose();
	}

	[OneTimeTearDown]
	public void OneTimeTearDown() {
		Directory.Delete( _testFolder, true );
	}

	[Test]
	public void PutMetadataAsync_ValidMetadata_NoExceptionsThrown() {
		IMutableFileMetadataRepository repo = _diskRepository;

		Id<FileMetadata> fileId = new Id<FileMetadata>( Guid.NewGuid() );
		FileMetadata fileMetadata = new FileMetadata( fileId, "name", "mime type", 1234L, DateTime.UtcNow );

		Assert.DoesNotThrowAsync( async () => { await repo.PutMetadataAsync( fileMetadata, CancellationToken.None ); } );
	}
}
