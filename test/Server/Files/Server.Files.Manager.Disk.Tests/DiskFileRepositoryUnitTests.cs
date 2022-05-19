using System.IO.Abstractions;
using Common.Files;
using Common.Files.Manager.Repositories;

namespace Server.Files.Manager.Disk.Tests;

[TestFixture]
public class DiskFileRepositoryUnitTests {

	private Mock<IFileFolderProvider> _fileFolderProvider;
	private Mock<IFileSystem> _fileSystem;
	private Mock<IFileStreamFactory> _fileStreamFactory;
	private DiskFileRepository _diskRepository;

	[SetUp]
	public void SetUp() {
		_fileFolderProvider = new Mock<IFileFolderProvider>( MockBehavior.Strict );
		_fileStreamFactory = new Mock<IFileStreamFactory>( MockBehavior.Strict );
		_fileSystem = new Mock<IFileSystem>( MockBehavior.Strict );
		_fileSystem
			.Setup( fs => fs.FileStream )
			.Returns( _fileStreamFactory.Object );

		_diskRepository = new DiskFileRepository(
			_fileFolderProvider.Object,
			_fileSystem.Object
		);
	}

	[Test]
	public void PutMetadataAsync_ValidMetadata_NoExceptionsThrown() {
		IMutableFileMetadataRepository repo = _diskRepository;
		string folder = @"does\not\exist";
		_fileFolderProvider
			.Setup( ffp => ffp.GetLocation() )
			.Returns( folder );

		Id<FileMetadata> fileId = new Id<FileMetadata>( Guid.NewGuid() );
		string filename = Path.Combine( folder, fileId.Value + ".metadata" );
		using MemoryStream output = new MemoryStream();
		_fileStreamFactory
			.Setup( fsf => fsf.Create( filename, FileMode.Create, FileAccess.Write, FileShare.None ) )
			.Returns( output );

		FileMetadata fileMetadata = new FileMetadata( fileId, "name", "mime type", 1234L, DateTime.UtcNow );

		Assert.DoesNotThrowAsync( async () => { await repo.PutMetadataAsync( fileMetadata, CancellationToken.None ); } );
	}
}
