using Common.Files;
using Common.Files.Manager;
using Common.Worlds;

namespace Common.Renderers;

public interface IWorldRenderer {
	Task RenderAtlasToAsync(
		IMutableFileManager fileManager,
		Id<FileMetadata> fileId,
		Buffer<TileTerrain> terrain,
		CancellationToken cancellationToken
	);

	Task RenderTerrainMapToAsync(
		IMutableFileManager fileManager,
		Id<FileMetadata> fileId,
		Buffer<TileTerrain> terrain,
		TileTerrain terrainToRender,
		CancellationToken cancellationToken
	);
}
