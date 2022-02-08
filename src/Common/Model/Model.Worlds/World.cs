namespace Model.Worlds;

public record World(
	Id<World> WorldId,
	string Name,
	string Seed,
	int Rows,
	int Columns,
	DateTime CreatedOn
);
