namespace Common.Core;

public sealed class Id<T> : IEquatable<Id<T>> {

	public static readonly Id<T> Empty = new Id<T>( Guid.Empty.ToString("N") );

	public static readonly IEnumerable<Id<T>> EmptyList = new List<Id<T>>();

	public Id( Guid id ) {
		if( id == Guid.Empty ) {
			throw new ArgumentException( "Parameter must not be empty GUID.", nameof( id ) );
		}

		Value = $"{typeof(T).Name}::{id:N}";
	}

	public Id( long id ) {
		if( id == default ) {
			throw new ArgumentException( "Parameter must not be default.", nameof( id ) );
		}

		Value = $"{typeof( T ).Name}::{id}";
	}

	public Id( string value ) {
		Value = value;
	}

	public string Value { get; init; }

	public override string ToString() {
		return Value;
	}

	public override int GetHashCode() {
		return HashCode.Combine( Value );
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
			Value.Equals( other.Value, StringComparison.Ordinal );
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
