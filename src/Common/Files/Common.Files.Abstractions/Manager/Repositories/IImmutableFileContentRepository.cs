namespace Common.Files.Manager.Repositories;

public interface IImmutableFileContentRepository {
	Task<bool> TryGetContentAsync(
		Id<FileMetadata> fileId,
		Func<Stream, Task> contentReader,
		CancellationToken cancellationToken
	);
}

