namespace Common.Files.Manager.Repositories;

public interface IFileContentRepository {
	Task<Stream> GetContentAsync(
		Id<FileMetadata> fileId,
		CancellationToken cancellationToken
	);
}

