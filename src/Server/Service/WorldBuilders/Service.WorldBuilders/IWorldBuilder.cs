namespace Service.WorldBuilders;

internal interface IWorldBuilder {

	void Build(
		string name,
		string seed,
		int rows,
		int columns
	);
}

