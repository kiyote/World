namespace Common.Manager.Files;

public sealed record FileMetadata(
	Id<FileMetadata> FileId,
	string Name,
	string MimeType,
	long Size,
	DateTime CreatedOn
);


