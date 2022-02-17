namespace Common.Manager.Worlds;

public record Tile(
	Id<Tile> TileId,
	Id<World> WorldId
);
