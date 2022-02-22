using Common.Files.Manager.Repositories;

namespace Server.Files.Manager.Disk;

internal interface IDiskFileRepository: IMutableFileContentRepository, IMutableFileMetadataRepository {
}

