namespace Common.Buffer;


[System.Diagnostics.CodeAnalysis.SuppressMessage( "Naming", "CA1716:Identifiers should not match keywords", Justification = "These are simple logical operators and these are the best names." )]
public interface IBufferOperator {
	IBufferOperator And( IBuffer<float> a, float thresholdA, IBuffer<float> b, float thresholdB, IBuffer<float> output );

	IBuffer<float> And( IBuffer<float> a, float thresholdA, IBuffer<float> b, float thresholdB );

	IBufferOperator Or( IBuffer<float> a, float thresholdA, IBuffer<float> b, float thresholdB, IBuffer<float> output );
	IBuffer<float> Or( IBuffer<float> a, float thresholdA, IBuffer<float> b, float thresholdB );

	IBufferOperator Subtract( IBuffer<float> source, IBuffer<float> amount, bool clamp, IBuffer<float> output );
	IBufferOperator Subtract( IBuffer<float> source, float amount, bool clamp, IBuffer<float> output );
	IBuffer<float> Subtract( IBuffer<float> source, IBuffer<float> amount, bool clamp );
	IBuffer<float> Subtract( IBuffer<float> source, float amount, bool clamp );

	IBufferOperator Add( IBuffer<float> source, float amount, bool clamp, IBuffer<float> output );
	IBufferOperator Add( IBuffer<float> source, IBuffer<float> amount, bool clamp, IBuffer<float> output );
	IBuffer<float> Add( IBuffer<float> source, float amount, bool clamp );
	IBuffer<float> Add( IBuffer<float> source, IBuffer<float> amount, bool clamp );

	IBufferOperator EdgeDetect( IBuffer<float> source, float threshold, IBuffer<float> output );
	IBuffer<float> EdgeDetect( IBuffer<float> source, float threshold );

	IBufferOperator Invert( IBuffer<float> source, IBuffer<float> output );
	IBuffer<float> Invert( IBuffer<float> source );

	IBufferOperator GateHigh( IBuffer<float> source, float threshold, IBuffer<float> output );
	IBuffer<float> GateHigh( IBuffer<float> source, float threshold );

	IBufferOperator GateLow( IBuffer<float> source, float threshold, IBuffer<float> output );
	IBuffer<float> GateLow( IBuffer<float> source, float threshold );

	IBufferOperator Range( IBuffer<float> source, float min, float max, IBuffer<float> output );
	IBuffer<float> Range( IBuffer<float> source, float min, float max );

	IBufferOperator Normalize( IBuffer<float> source, IBuffer<float> output );
	IBuffer<float> Normalize( IBuffer<float> source );

	IBufferOperator Multiply( IBuffer<float> source, float amount, bool clamp, IBuffer<float> output );
	IBufferOperator Multiply( IBuffer<float> source, IBuffer<float> amount, bool clamp, IBuffer<float> output );
	IBuffer<float> Multiply( IBuffer<float> source, float amount, bool clamp );
	IBuffer<float> Multiply( IBuffer<float> source, IBuffer<float> amount, bool clamp );

	IBufferOperator Threshold( IBuffer<float> source, float mininum, float minimumValue, float maximum, float maximumValue, IBuffer<float> output );
	IBuffer<float> Threshold( IBuffer<float> source, float mininum, float minimumValue, float maximum, float maximumValue );

	IBufferOperator Quantize( IBuffer<float> source, float[] ranges, IBuffer<float> output );
	IBuffer<float> Quantize( IBuffer<float> source, float[] ranges );

	IBufferOperator Denoise( IBuffer<float> source, IBuffer<float> output );
	IBuffer<float> Denoise( IBuffer<float> source );

	IBufferOperator Mask( IBuffer<float> source, IBuffer<float> mask, float value, IBuffer<float> output );
	IBuffer<float> Mask( IBuffer<float> source, IBuffer<float> mask, float value );
}
