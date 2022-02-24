namespace Common.Worlds.Builder.Noises;

public interface ILogicalOperator {
	float[,] PerformAnd( ref float[,] a, float thresholdA, ref float[,] b, float thresholdB );
	float[,] PerformOr( ref float[,] a, float thresholdA, ref float[,] b, float thresholdB );
}
