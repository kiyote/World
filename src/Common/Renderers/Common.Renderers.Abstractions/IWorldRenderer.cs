using Common.Files;
using Common.Files.Manager;
using Common.Worlds;

namespace Common.Renderers;

public interface IWorldRenderer {
	Task RenderTerrainToAsync(
		IMutableFileManager fileSystem,
		Id<FileMetadata> fileId,
		TileTerrain[,] terrain,
		CancellationToken cancellationToken
	);
}
