namespace World.Core;

public sealed class Id<T> : IEquatable<Id<T>> {

	public static readonly Id<T> Empty = new Id<T>( Guid.Empty.ToString( "N" ), false );

	public static readonly IEnumerable<Id<T>> EmptyList = new List<Id<T>>();

	public Id() {
		Value = Guid.NewGuid();
	}

	public Id( Guid id ) {
		if( id == Guid.Empty ) {
			throw new ArgumentException( "Parameter must not be empty GUID.", nameof( id ) );
		}

		Value = id;
	}

	public Id( string value ) :
		this( value, false ) {
	}

	private Id( string value, bool validate ) {
		if( !validate ) {
			Value = Guid.Parse( value );
		} else {
			if( string.IsNullOrWhiteSpace( value ) ) {
				throw new ArgumentException( value );
			}

			if( !Guid.TryParse( value, out Guid id ) ) {
				throw new ArgumentException( "Parameter must be a valid GUID.", nameof( value ) );
			}

			if( id == Guid.Empty ) {
				throw new ArgumentException( "Parameter must not be empty GUID.", nameof( value ) );
			}

			Value = id;
		}

	}

	public Guid Value { get; init; }

	public override string ToString() {
		return Value.ToString( "N" );
	}

	public override int GetHashCode() {
		return Value.GetHashCode();
	}

	public override bool Equals( object? obj ) {
		if( obj is not Id<T> target ) {
			return false;
		}

		return Equals( target );
	}

	public bool Equals( Id<T>? other ) {
		if( other is null ) {
			return false;
		}

		if( ReferenceEquals( other, this ) ) {
			return true;
		}

		return
			Value == other.Value;
	}

	public static bool operator ==( Id<T>? left, Id<T>? right ) {
		if (left is null) {
			return ( right is null );
		}

		return
			left.Equals( right );
	}

	public static bool operator !=( Id<T> left, Id<T> right ) {
		return !( left == right );
	}
}
