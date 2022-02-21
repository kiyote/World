namespace Common.Files.Manager.Resource;

public interface IResourceFileManager: IImmutableFileManager {
	public Id<FileMetadata> MountainTerrainId { get; }
	public Id<FileMetadata> HillTerrainId { get; }
	public Id<FileMetadata> PlainsTerrainId { get; }
}

