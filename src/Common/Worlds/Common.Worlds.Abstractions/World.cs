namespace Common.Worlds;

public record World(
	Id<World> WorldId,
	string Name,
	string Seed,
	ISize Size,
	DateTime CreatedOn
);
