namespace Common.Files.Manager;

public interface IImmutableFileManager {

	Task<bool> TryGetContentAsync(
		Id<FileMetadata> fileId,
		Func<Stream, Task> contentReader,
		CancellationToken cancellationToken
	);

	Task<FileMetadata> GetMetadataAsync(
		Id<FileMetadata> fileId,
		CancellationToken cancellationToken
	);
}
