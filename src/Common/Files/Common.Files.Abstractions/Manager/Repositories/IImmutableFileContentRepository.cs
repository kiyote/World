namespace Common.Files.Manager.Repositories;

public interface IImmutableFileContentRepository {
	Task<bool> TryGetContentAsync(
		Id<FileMetadata> fileId,
		AsyncStreamHandler contentReader,
		CancellationToken cancellationToken
	);
}

