namespace Common.Files.Manager.Resource;

public interface IResourceFileManager: IImmutableFileManager {
	public Id<FileMetadata> MountainTileId { get; }
	public Id<FileMetadata> HillTileId { get; }
	public Id<FileMetadata> ForestTileId { get; }
	public Id<FileMetadata> GrassTileId { get; }
	public Id<FileMetadata> CoastTileId { get; }
	public Id<FileMetadata> OceanTileId { get; }
}

