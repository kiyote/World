using Kiyote.Buffers;

namespace Common.Worlds.Builder;

public sealed record WorldMaps(
	IBuffer<TileTerrain> Terrain,
	IBuffer<TileFeature> Feature
);

