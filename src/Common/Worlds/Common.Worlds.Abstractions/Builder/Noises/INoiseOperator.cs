namespace Common.Worlds.Builder.Noises;

[System.Diagnostics.CodeAnalysis.SuppressMessage( "Naming", "CA1716:Identifiers should not match keywords", Justification = "Tough.  Suck it up other languages." )]
public interface INoiseOperator {
	INoiseOperator And( Buffer<float> a, float thresholdA, Buffer<float> b, float thresholdB, Buffer<float> output );
	Buffer<float> And( Buffer<float> a, float thresholdA, Buffer<float> b, float thresholdB );

	INoiseOperator Or( Buffer<float> a, float thresholdA, Buffer<float> b, float thresholdB, Buffer<float> output );
	Buffer<float> Or( Buffer<float> a, float thresholdA, Buffer<float> b, float thresholdB );

	INoiseOperator Subtract( Buffer<float> source, Buffer<float> amount, bool clamp, Buffer<float> output );
	INoiseOperator Subtract( Buffer<float> source, float amount, bool clamp, Buffer<float> output );
	Buffer<float> Subtract( Buffer<float> source, Buffer<float> amount, bool clamp );
	Buffer<float> Subtract( Buffer<float> source, float amount, bool clamp );

	INoiseOperator Add( Buffer<float> source, float amount, bool clamp, Buffer<float> output );
	INoiseOperator Add( Buffer<float> source, Buffer<float> amount, bool clamp, Buffer<float> output );
	Buffer<float> Add( Buffer<float> source, float amount, bool clamp );
	Buffer<float> Add( Buffer<float> source, Buffer<float> amount, bool clamp );

	INoiseOperator EdgeDetect( Buffer<float> source, float threshold, Buffer<float> output );
	Buffer<float> EdgeDetect( Buffer<float> source, float threshold );

	INoiseOperator Invert( Buffer<float> source, Buffer<float> output );
	Buffer<float> Invert( Buffer<float> source );

	INoiseOperator GateHigh( Buffer<float> source, float threshold, Buffer<float> output );
	Buffer<float> GateHigh( Buffer<float> source, float threshold );

	INoiseOperator GateLow( Buffer<float> source, float threshold, Buffer<float> output );
	Buffer<float> GateLow( Buffer<float> source, float threshold );

	INoiseOperator Range( Buffer<float> source, float min, float max, Buffer<float> output );
	Buffer<float> Range( Buffer<float> source, float min, float max );

	INoiseOperator Normalize( Buffer<float> source, Buffer<float> output );
	Buffer<float> Normalize( Buffer<float> source );

	INoiseOperator Multiply( Buffer<float> source, float amount, bool clamp, Buffer<float> output );
	INoiseOperator Multiply( Buffer<float> source, Buffer<float> amount, bool clamp, Buffer<float> output );
	Buffer<float> Multiply( Buffer<float> source, float amount, bool clamp );
	Buffer<float> Multiply( Buffer<float> source, Buffer<float> amount, bool clamp );

	INoiseOperator Threshold( Buffer<float> source, float mininum, float minimumValue, float maximum, float maximumValue, Buffer<float> output );
	Buffer<float> Threshold( Buffer<float> source, float mininum, float minimumValue, float maximum, float maximumValue );

	INoiseOperator Quantize( Buffer<float> source, float[] ranges, Buffer<float> output );
	Buffer<float> Quantize( Buffer<float> source, float[] ranges );

	INoiseOperator Denoise( Buffer<float> source, Buffer<float> output );
	Buffer<float> Denoise( Buffer<float> source );
}

