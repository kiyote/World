namespace Common.Files.Manager.Repositories;

public interface IImmutableFileMetadataRepository {
	Task<FileMetadata> GetMetadataAsync(
		Id<FileMetadata> fileId,
		CancellationToken cancellationToken
	);
}

