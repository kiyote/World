namespace Common.Worlds;

public record Tile(
	Id<Tile> TileId,
	Id<World> WorldId
);
