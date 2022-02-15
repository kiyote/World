﻿namespace Server.Service.WorldBuilders;

public interface INoiseProvider {

	float[,] Generate(
		long seed,
		int rows,
		int columns,
		float frequency
	);
}

