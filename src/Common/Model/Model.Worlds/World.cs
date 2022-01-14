namespace Common.Model.Worlds;

public record World(
	Id<World> Id,
	string Name,
	DateTime CreatedOn
);
