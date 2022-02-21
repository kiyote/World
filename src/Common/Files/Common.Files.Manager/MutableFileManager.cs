using Common.Files.Manager.Repositories;

namespace Common.Files.Manager;

[System.Diagnostics.CodeAnalysis.SuppressMessage( "Performance", "CA1812:An internal (assembly-level) type is never instantiated.", Justification = "This class is instantiated via DI." )]
public abstract class MutableFileManager<TFileContentRepository, TFileMetadataRepository> : ImmutableFileManager<TFileContentRepository, TFileMetadataRepository>, IMutableFileManager
	where TFileContentRepository : IMutableFileContentRepository
	where TFileMetadataRepository : IMutableFileMetadataRepository {

	private readonly TFileContentRepository _fileContentRepository;
	private readonly TFileMetadataRepository _fileMetadataRepository;

	protected MutableFileManager(
		TFileContentRepository fileContentRepository,
		TFileMetadataRepository fileMetadataRepository
	): base(
		fileContentRepository,
		fileMetadataRepository
	) {
		_fileContentRepository = fileContentRepository;
		_fileMetadataRepository = fileMetadataRepository;
	}

	public Task PutMetadataAsync(
		FileMetadata fileMetadata,
		CancellationToken cancellationToken
	) {
		return _fileMetadataRepository.PutMetadataAsync( fileMetadata, cancellationToken );
	}

	public Task PutContentAsync(
		Id<FileMetadata> fileId,
		Func<Stream, Task> asyncWriter,
		CancellationToken cancellationToken
	) {
		return _fileContentRepository.PutContentAsync( fileId, asyncWriter, cancellationToken );
	}
}

