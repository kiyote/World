using Common.Core;

namespace Common.Geometry;

public interface IPointFactory {

	IReadOnlyList<IPoint> Random( int count, Size bounds, int distanceApart );
}
