namespace Common.Manager.Files.Repositories;

public sealed record FileMetadata(
	Id<FileMetadata> FileId,
	string Name,
	string MimeType,
	long Size,
	DateTime CreatedOn
);


