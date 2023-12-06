
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

		Assert.That( fileMetadata, Is.Not.Null );
		Assert.That( filename, Is.EqualTo( fileMetadata.Name ) );
		Assert.That( default( long ), Is.Not.EqualTo( fileMetadata.Size ) );
		Assert.That( "image/png", Is.EqualTo( fileMetadata.MimeType ) );
		Assert.That( DateTime.UtcNow, Is.GreaterThanOrEqualTo( fileMetadata.CreatedOn ) );
	}
}
