namespace World.Repository.Actors;

public interface IActorRepository
{
	Task<Actor> CreateAsync(
		Id<Actor> id,
		string name,
		CancellationToken cancellationToken
	);
}
