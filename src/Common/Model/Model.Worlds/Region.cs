namespace Model.Worlds;

public record Region(
	Id<Region> RegionId,
	Id<World> WorldId,
	string Name,
	IEnumerable<Id<Tile>> TileIds
);
