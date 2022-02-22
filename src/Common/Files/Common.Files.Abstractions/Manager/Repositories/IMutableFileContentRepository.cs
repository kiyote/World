namespace Common.Files.Manager.Repositories;

public interface IMutableFileContentRepository: IImmutableFileContentRepository {

	Task PutContentAsync(
		Id<FileMetadata> fileId,
		AsyncStreamHandler asyncWriter,
		CancellationToken cancellationToken
	);
}
