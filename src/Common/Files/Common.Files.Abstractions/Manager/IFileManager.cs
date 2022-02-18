using Common.Files.Manager.Repositories;

namespace Common.Files.Manager;

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

