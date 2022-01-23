namespace Common.Model.Worlds;

public record RegionChunk(
	Id<RegionChunk> RegionChunkId,
	Id<Region> RegionId,
	IEnumerable<Id<Tile>> TileIds
);
