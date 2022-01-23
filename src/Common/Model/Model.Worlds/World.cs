namespace Common.Model.Worlds;

public record World(
	Id<World> WorldId,
	string Name,
	DateTime CreatedOn
);
