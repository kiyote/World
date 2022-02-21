namespace Common.Files.Manager.Resource;

[System.Diagnostics.CodeAnalysis.SuppressMessage( "Performance", "CA1812:An internal (assembly-level) type is never instantiated.", Justification = "This class is instantiated via DI." )]
internal class ResourceFileManager : ImmutableFileManager<ResourceFileRepository, ResourceFileRepository>, IResourceFileManager {

	internal static ResourceFileManager GetInstance() {
		ResourceFileRepository repository = new ResourceFileRepository();
		return new ResourceFileManager(
			repository,
			repository
		);
	}

	private readonly static Id<FileMetadata> _mountainTerrainId = new Id<FileMetadata>( "hex_mountain.png" );
	private readonly static Id<FileMetadata> _hillTerrainId = new Id<FileMetadata>( "hex_hill.png" );
	private readonly static Id<FileMetadata> _plainsTerrainId = new Id<FileMetadata>( "hex_plains.png" );

	public ResourceFileManager(
		ResourceFileRepository fileContentRepository,
		ResourceFileRepository fileMetadataRepository
	) : base(
		fileContentRepository,
		fileMetadataRepository
	) {
	}

	Id<FileMetadata> IResourceFileManager.MountainTerrainId => _mountainTerrainId;

	Id<FileMetadata> IResourceFileManager.HillTerrainId => _hillTerrainId;

	Id<FileMetadata> IResourceFileManager.PlainsTerrainId => _plainsTerrainId;
}

