
namespace Common.Files.Manager.Resource.Tests;

[TestFixture]
public class ResourceFileRepositoryIntegrationTests {

	private IResourceFileRepository _repo;

    [SetUp]
    public void SetUp() {
		_repo = new ResourceFileRepository();
    }

    [Test]
    public async Task GetMetadata()
    {
		string filename = "tile_mountain.png";
		Id<FileMetadata> fileId = new Id<FileMetadata>( filename );
		FileMetadata fileMetadata = await _repo.GetMetadataAsync( fileId, CancellationToken.None ).ConfigureAwait( false );

		Assert.IsNotNull( fileMetadata );
		Assert.AreEqual( filename, fileMetadata.Name );
		Assert.AreNotEqual( default( long ), fileMetadata.Size );
		Assert.AreEqual( "image/png", fileMetadata.MimeType );
		Assert.GreaterOrEqual( DateTime.UtcNow, fileMetadata.CreatedOn );
	}
}
