namespace Common.Files.Manager.Repositories;

public interface IFileMetadataRepository {
	Task<FileMetadata> GetMetadataAsync(
		Id<FileMetadata> fileId,
		CancellationToken cancellationToken
	);

	Task PutMetadataAsync(
		Id<FileMetadata> fileId,
		FileMetadata fileMetadata,
		CancellationToken cancellationToken
	);
}

