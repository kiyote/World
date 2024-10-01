using Common.Files;
using Common.Files.Manager;
using Common.Worlds;
using Kiyote.Buffers;

namespace Common.Renderers;

public interface IWorldRenderer {
	Task RenderAtlasToAsync(
		IMutableFileManager fileManager,
		Id<FileMetadata> fileId,
		IBuffer<TileTerrain> terrain,
		CancellationToken cancellationToken
	);

	Task RenderTerrainMapToAsync(
		IMutableFileManager fileManager,
		Id<FileMetadata> fileId,
		IBuffer<TileTerrain> terrain,
		TileTerrain terrainToRender,
		CancellationToken cancellationToken
	);
}
