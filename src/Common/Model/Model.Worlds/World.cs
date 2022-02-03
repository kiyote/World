namespace Common.Model.Worlds;

public record World(
	Id<World> WorldId,
	string Name,
	string Seed,
	DateTime CreatedOn
);
