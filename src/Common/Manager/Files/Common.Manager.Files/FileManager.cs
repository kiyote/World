using Common.Manager.Files.Repositories;

namespace Common.Manager.Files;

[System.Diagnostics.CodeAnalysis.SuppressMessage( "Performance", "CA1812:An internal (assembly-level) type is never instantiated.", Justification = "This class is instantiated via DI." )]
internal sealed class FileManager : IFileManager {

	private readonly IFileContentRepository _fileContentRepository;
	private readonly IFileMetadataRepository _fileMetadataRepository;

	public FileManager(
		IFileContentRepository fileContentRepository,
		IFileMetadataRepository fileMetadataRepository
	) {
		_fileContentRepository = fileContentRepository;
		_fileMetadataRepository = fileMetadataRepository;
	}

	Task<Stream> IFileManager.GetContentAsync(
		Id<FileMetadata> fileId,
		CancellationToken cancellationToken
	) {
		return _fileContentRepository.GetContentAsync( fileId, cancellationToken );
	}

	Task<FileMetadata> IFileManager.GetMetadataAsync(
		Id<FileMetadata> fileId,
		CancellationToken cancellationToken
	) {
		return _fileMetadataRepository.GetMetadataAsync( fileId, cancellationToken );
	}
}

