namespace Common.Worlds.Builder.Noises;

[System.Diagnostics.CodeAnalysis.SuppressMessage( "Naming", "CA1716:Identifiers should not match keywords", Justification = "Tough.  Suck it up other languages." )]
public interface INoiseOperator {
	float[,] And( ref float[,] a, float thresholdA, ref float[,] b, float thresholdB );
	float[,] Or( ref float[,] a, float thresholdA, ref float[,] b, float thresholdB );
	float[,] Subtract( ref float[,] source, ref float[,] amount, bool clamp );
	float[,] Subtract( ref float[,] source, float amount, bool clamp );
	float[,] Add( ref float[,] source, float amount, bool clamp );
	float[,] Add( ref float[,] source, ref float[,] amount, bool clamp );
	float[,] EdgeDetect( ref float[,] source, float threshold );
	float[,] Invert( ref float[,] source );
	float[,] GateHigh( ref float[,] source, float threshold );
	float[,] GateLow( ref float[,] source, float threshold );
	float[,] Range( ref float[,] source, float min, float max );
	float[,] Normalize( ref float[,] source );
	float[,] Multiply( ref float[,] source, float amount, bool clamp );
	float[,] Multiply( ref float[,] source, ref float[,] amount, bool clamp );
	float[,] Threshold( ref float[,] source, float mininum, float minimumValue, float maximum, float maximumValue );
}

