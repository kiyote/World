namespace Common.Model.Worlds;

public record Tile(
	Id<Tile> TileId,
	Id<World> WorldId
);
