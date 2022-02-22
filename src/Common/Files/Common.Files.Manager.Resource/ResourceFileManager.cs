namespace Common.Files.Manager.Resource;

[System.Diagnostics.CodeAnalysis.SuppressMessage( "Performance", "CA1812:An internal (assembly-level) type is never instantiated.", Justification = "This class is instantiated via DI." )]
internal class ResourceFileManager : ImmutableFileManager<IResourceFileRepository, IResourceFileRepository>, IResourceFileManager {

	private readonly static Id<FileMetadata> _mountainTerrainId = new Id<FileMetadata>( "hex_mountain.png" );
	private readonly static Id<FileMetadata> _hillTerrainId = new Id<FileMetadata>( "hex_hill.png" );
	private readonly static Id<FileMetadata> _plainsTerrainId = new Id<FileMetadata>( "hex_plains.png" );

	public ResourceFileManager(
		IResourceFileRepository fileContentRepository,
		IResourceFileRepository fileMetadataRepository
	) : base(
		fileContentRepository,
		fileMetadataRepository
	) {
	}

	Id<FileMetadata> IResourceFileManager.MountainTerrainId => _mountainTerrainId;

	Id<FileMetadata> IResourceFileManager.HillTerrainId => _hillTerrainId;

	Id<FileMetadata> IResourceFileManager.PlainsTerrainId => _plainsTerrainId;
}

