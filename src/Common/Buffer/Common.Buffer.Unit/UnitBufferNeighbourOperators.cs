namespace Common.Buffer.Unit;

[System.Diagnostics.CodeAnalysis.SuppressMessage( "Performance", "CA1812:An internal (assembly-level) type is never instantiated.", Justification = "This class is instantiated via DI." )]
internal sealed class UnitBufferNeighbourOperators : IUnitBufferNeighbourOperators {

	private readonly IBufferFactory _bufferFactory;
	private readonly INeighbourLocator _neighbourLocator;

	public UnitBufferNeighbourOperators(
		IBufferFactory bufferFactory,
		INeighbourLocator neighbourLocator
	) {
		_bufferFactory = bufferFactory;
		_neighbourLocator = neighbourLocator;
	}


	IBufferNeighbourOperators<float> IBufferNeighbourOperators<float>.Denoise(
		IBuffer<float> source,
		IBuffer<float> output
	) {
		int rows = source.Size.Rows;
		int columns = source.Size.Columns;

		DoSingleRangeOperator(
			source,
			( int row, int column, IBuffer<float> input, float value ) => {
				IEnumerable<Location> neighbours = _neighbourLocator.GetNeighbours( source.Size, column, row );
				IEnumerable<float> values = neighbours.Select( neighbour => input[neighbour.Row][neighbour.Column] );
				values = values.Distinct();
				if( values.Count() == 1 ) {
					return values.First();
				}
				return value;
			},
			output
		);
		return this;
	}

	IBuffer<float> IBufferNeighbourOperators<float>.Denoise(
		IBuffer<float> source
	) {
		IBuffer<float> output = _bufferFactory.Create<float>( source.Size );
		( this as IUnitBufferNeighbourOperators ).Denoise( source, output );
		return output;
	}

	IBufferNeighbourOperators<float> IBufferNeighbourOperators<float>.EdgeDetect(
		IBuffer<float> source,
		float threshold,
		IBuffer<float> output
	) {
		int rows = source.Size.Rows;
		int columns = source.Size.Columns;

		DoSingleRangeOperator(
			source,
			( int row, int column, IBuffer<float> input, float value ) => {
				if( value == 0.0f ) {
					bool isEdge = false;
					IEnumerable<Location> neighbours = _neighbourLocator.GetNeighbours( source.Size, column, row );
					foreach( Location location in neighbours ) {
						if( input[location.Row][location.Column] > threshold ) {
							isEdge = true;
						}
					}
					return isEdge ? 1.0f : 0.0f;
				}
				return 0.0f;
			},
			output
		);
		return this;
	}

	IBuffer<float> IBufferNeighbourOperators<float>.EdgeDetect(
		IBuffer<float> source,
		float threshold
	) {
		IBuffer<float> output = _bufferFactory.Create<float>( source.Size );
		( this as IUnitBufferNeighbourOperators ).EdgeDetect( source, threshold, output );
		return output;
	}

	private static void DoSingleRangeOperator(
		IBuffer<float> a,
		Func<int, int, IBuffer<float>, float, float> op,
		IBuffer<float> output
	) {
		int rows = a.Size.Rows;
		int columns = a.Size.Columns;

		for( int r = 0; r < rows; r++ ) {
			for( int c = 0; c < columns; c++ ) {
				output[r][c] = op( c, r, a, a[r][c] );
			}
		}
	}
}
