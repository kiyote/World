using Common.Manager.Files.Repositories;

namespace Common.Manager.Files;

public interface IFileManager {
	Task<Stream> GetContentAsync(
		Id<FileMetadata> fileId,
		CancellationToken cancellationToken
	);

	Task<FileMetadata> GetMetadataAsync(
		Id<FileMetadata> fileId,
		CancellationToken cancellationToken
	);
}

