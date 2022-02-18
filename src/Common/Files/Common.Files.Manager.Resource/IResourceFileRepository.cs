using Common.Files.Manager.Repositories;

namespace Common.Files.Manager.Resource;

internal interface IResourceFileRepository : IImmutableFileContentRepository, IImmutableFileMetadataRepository {
}
