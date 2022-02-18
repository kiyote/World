namespace Common.Files.Manager.Resource;

[System.Diagnostics.CodeAnalysis.SuppressMessage( "Performance", "CA1812:An internal (assembly-level) type is never instantiated.", Justification = "This class is instantiated via DI." )]
internal class ResourceFileManager : ImmutableFileManager<ResourceFileRepository, ResourceFileRepository>, IResourceFileManager {
	public ResourceFileManager(
		ResourceFileRepository fileContentRepository,
		ResourceFileRepository fileMetadataRepository
	) : base(
		fileContentRepository,
		fileMetadataRepository
	) {
	}
}

