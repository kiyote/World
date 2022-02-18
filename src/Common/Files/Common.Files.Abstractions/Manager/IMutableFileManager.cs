namespace Common.Files.Manager;

public interface IMutableFileManager: IImmutableFileManager {

	Task PutMetadataAsync(
		FileMetadata fileMetadata,
		CancellationToken cancellationToken
	);
}
