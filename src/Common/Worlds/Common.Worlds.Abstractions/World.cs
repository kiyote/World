namespace Common.Worlds;

public record World(
	Id<World> WorldId,
	string Name,
	string Seed,
	Size Size,
	DateTime CreatedOn
);
