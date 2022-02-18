namespace Common.Files.Manager.Repositories;

public interface IMutableFileMetadataRepository: IImmutableFileMetadataRepository {
	Task PutMetadataAsync(
		FileMetadata fileMetadata,
		CancellationToken cancellationToken
	);
}

