namespace Common.Model.Players;

public record Player(
	Id<Player> PlayerId,
	string Name,
	DateTime CreatedOn
);
