namespace Common.Manager.Files.Repositories;

public interface IFileContentRepository {
	Task<Stream> GetContentAsync(
		Id<FileMetadata> fileId,
		CancellationToken cancellationToken
	);
}

