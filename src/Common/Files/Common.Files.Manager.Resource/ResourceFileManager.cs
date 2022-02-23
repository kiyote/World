namespace Common.Files.Manager.Resource;

[System.Diagnostics.CodeAnalysis.SuppressMessage( "Performance", "CA1812:An internal (assembly-level) type is never instantiated.", Justification = "This class is instantiated via DI." )]
internal class ResourceFileManager : ImmutableFileManager<IResourceFileRepository, IResourceFileRepository>, IResourceFileManager {

	private readonly static Id<FileMetadata> _mountainTileId = new Id<FileMetadata>( "tile_mountain.png" );
	private readonly static Id<FileMetadata> _hillTileId = new Id<FileMetadata>( "tile_hill.png" );
	private readonly static Id<FileMetadata> _grassTileId = new Id<FileMetadata>( "tile_grass.png" );
	private readonly static Id<FileMetadata> _forestTileId = new Id<FileMetadata>( "tile_forest.png" );
	private readonly static Id<FileMetadata> _coastTileId = new Id<FileMetadata>( "tile_coast.png" );
	private readonly static Id<FileMetadata> _oceanTileId = new Id<FileMetadata>( "tile_ocean.png" );

	public ResourceFileManager(
		IResourceFileRepository fileContentRepository,
		IResourceFileRepository fileMetadataRepository
	) : base(
		fileContentRepository,
		fileMetadataRepository
	) {
	}

	Id<FileMetadata> IResourceFileManager.MountainTileId => _mountainTileId;

	Id<FileMetadata> IResourceFileManager.HillTileId => _hillTileId;

	Id<FileMetadata> IResourceFileManager.GrassTileId => _grassTileId;

	Id<FileMetadata> IResourceFileManager.ForestTileId => _forestTileId;

	Id<FileMetadata> IResourceFileManager.CoastTileId => _coastTileId;

	Id<FileMetadata> IResourceFileManager.OceanTileId => _oceanTileId;
}

