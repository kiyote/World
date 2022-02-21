namespace Common.Files.Manager.Repositories;

public interface IMutableFileContentRepository: IImmutableFileContentRepository {

	Task PutContentAsync(
		Id<FileMetadata> fileId,
		Func<Stream, Task> asyncWriter,
		CancellationToken cancellationToken
	);
}
