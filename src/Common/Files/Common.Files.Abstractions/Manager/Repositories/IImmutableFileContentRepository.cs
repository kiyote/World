namespace Common.Files.Manager.Repositories;

public interface IImmutableFileContentRepository {
	Task<Stream> GetContentAsync(
		Id<FileMetadata> fileId,
		CancellationToken cancellationToken
	);
}

