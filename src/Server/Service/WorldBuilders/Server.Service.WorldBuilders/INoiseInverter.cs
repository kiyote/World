namespace Server.Service.WorldBuilders;

public interface INoiseInverter {

	float[,] Invert( float[,] source );
}

