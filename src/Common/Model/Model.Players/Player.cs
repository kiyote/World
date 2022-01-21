namespace Common.Model.Players;

public record Player(
	Id<Player> Id,
	string Name,
	DateTime CreatedOn
);
