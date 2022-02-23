namespace Common.Worlds;

public record Tile(
	Id<Tile> TileId,
	TileTerrain Terrain,
	IEnumerable<TileFeature> Features
);
