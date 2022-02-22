namespace Common.Files.Manager;

public interface IMutableFileManager: IImmutableFileManager {

	Task PutMetadataAsync(
		FileMetadata fileMetadata,
		CancellationToken cancellationToken
	);

	Task PutContentAsync(
		Id<FileMetadata> fileId,
		AsyncStreamHandler asyncWriter,
		CancellationToken cancellationToken
	);
}
