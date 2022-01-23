using Common.DynamoDb;

namespace Manager.Worlds.Repositories.DynamoDb;

[System.Diagnostics.CodeAnalysis.SuppressMessage( "Performance", "CA1812:An internal (assembly-level) type is never instantiated.", Justification = "This class is instantiated via DI." )]
internal sealed class DynamoDbRegionRepository : IRegionRepository {

	private readonly IWorldDynamoDbRepository _db;

	public DynamoDbRegionRepository(
		IWorldDynamoDbRepository db
	) {
		_db = db;
	}

	async Task<Region> IRegionRepository.CreateAsync(
		Id<World> worldId,
		Id<Region> regionId,
		string name,
		IEnumerable<Id<RegionChunk>> chunks,
		DateTime createdOn,
		CancellationToken cancellationToken
	) {
		var record = new RegionRecord(
			worldId,
			regionId,
			name,
			chunks,
			createdOn.ToUniversalTime()
		);

		await _db.CreateAsync( record, cancellationToken ).ConfigureAwait( false );

		return new Region(
			regionId,
			worldId,
			name,
			chunks
		);
	}

	async Task<Region?> IRegionRepository.GetByIdAsync(
		Id<World> worldId,
		Id<Region> regionId,
		CancellationToken cancellationToken
	) {
		RegionRecord? record = await _db.LoadAsync<RegionRecord?>(
			worldId.Value,
			worldId.Value,
			cancellationToken
		).ConfigureAwait( false );

		if( record is null ) {
			return null;
		}

		return new Region(
			regionId,
			worldId,
			record.Name,
			record.Chunks
		);

	}
}
