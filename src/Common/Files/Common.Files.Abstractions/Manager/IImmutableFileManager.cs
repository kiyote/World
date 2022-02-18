namespace Common.Files.Manager;

public interface IImmutableFileManager {

	Task<Stream> GetContentAsync(
		Id<FileMetadata> fileId,
		CancellationToken cancellationToken
	);

	Task<FileMetadata> GetMetadataAsync(
		Id<FileMetadata> fileId,
		CancellationToken cancellationToken
	);
}

