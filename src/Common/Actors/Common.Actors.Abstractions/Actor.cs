namespace Common.Actors;

public record Actor(
	Id<Actor> ActorId,
	string Name,
	DateTime CreatedOn
);

