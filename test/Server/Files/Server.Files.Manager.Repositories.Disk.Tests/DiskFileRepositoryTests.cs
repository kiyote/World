using Common.Files.Manager.Repositories;
using NUnit.Framework;

namespace Server.Files.Manager.Repositories.Disk.Tests;

[TestFixture]
public class DiskFileRepositoryTests {

	private string _testFolder;
	private DiskFileRepository _diskRepository;

	[SetUp]
	public void SetUp() {
		string root = Path.GetTempPath();
		_testFolder = Path.Combine( root, Guid.NewGuid().ToString( "N" ) );
		Directory.CreateDirectory( _testFolder );
		_diskRepository = new DiskFileRepository( _testFolder );
	}

	[TearDown]
	public void TearDown() {
		Directory.Delete( _testFolder, true );
	}

	[Test]
	public void PutMetadataAsync_ValidMetadata_NoExceptionsThrown() {
		IFileMetadataRepository repo = _diskRepository;

		Id<FileMetadata> fileId = new Id<FileMetadata>(Guid.NewGuid());
		FileMetadata fileMetadata = new FileMetadata( fileId, "name", "mime type", 1234L, DateTime.UtcNow );

		Assert.DoesNotThrowAsync( async () => { await repo.PutMetadataAsync( fileId, fileMetadata, CancellationToken.None ); } );
	}
}
