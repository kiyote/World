using Common.Files.Manager.Repositories;

namespace Common.Files.Manager;

[System.Diagnostics.CodeAnalysis.SuppressMessage( "Performance", "CA1812:An internal (assembly-level) type is never instantiated.", Justification = "This class is instantiated via DI." )]
public abstract class ImmutableFileManager<TFileContentRepository, TFileMetadataRepository> : IImmutableFileManager
	where TFileContentRepository: IImmutableFileContentRepository
	where TFileMetadataRepository: IImmutableFileMetadataRepository
{

	private readonly TFileContentRepository _fileContentRepository;
	private readonly TFileMetadataRepository _fileMetadataRepository;

	protected ImmutableFileManager(
		TFileContentRepository fileContentRepository,
		TFileMetadataRepository fileMetadataRepository
	) {
		_fileContentRepository = fileContentRepository;
		_fileMetadataRepository = fileMetadataRepository;
	}

	public Task<bool> TryGetContentAsync(
		Id<FileMetadata> fileId,
		AsyncStreamHandler contentReader,
		CancellationToken cancellationToken
	) {
		return _fileContentRepository.TryGetContentAsync( fileId, contentReader, cancellationToken );
	}

	public Task<FileMetadata> GetMetadataAsync(
		Id<FileMetadata> fileId,
		CancellationToken cancellationToken
	) {
		return _fileMetadataRepository.GetMetadataAsync( fileId, cancellationToken );
	}
}

