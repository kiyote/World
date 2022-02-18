using System.Text.Json;
using Common.Files.Manager.Repositories;

namespace Server.Files.Manager.Disk;

[System.Diagnostics.CodeAnalysis.SuppressMessage( "Performance", "CA1812:An internal (assembly-level) type is never instantiated.", Justification = "This class is instantiated via DI." )]
internal class DiskFileRepository : IMutableFileContentRepository, IMutableFileMetadataRepository {

	private readonly string _fileFolder;
	private readonly JsonSerializerOptions _options;

	public DiskFileRepository(
		string fileFolder
	) {
		if( !Directory.Exists( fileFolder ) ) {
			throw new InvalidOperationException( $"File folder '{fileFolder}' must exist." );
		}
		_fileFolder = fileFolder;
		_options = new JsonSerializerOptions() {
			AllowTrailingCommas = true,
			ReadCommentHandling = JsonCommentHandling.Skip,
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString,
			IgnoreReadOnlyFields = true,
			PropertyNameCaseInsensitive = true,
			WriteIndented = true
		};
	}

	Task<Stream> IImmutableFileContentRepository.GetContentAsync(
		Id<FileMetadata> fileId,
		CancellationToken cancellationToken
	) {
		string filename = Path.Combine( _fileFolder, fileId.Value );
		FileStream fs = new FileStream( filename, FileMode.Open, FileAccess.Read, FileShare.Read );
		return Task.FromResult( (Stream)fs );
	}

	async Task<FileMetadata> IImmutableFileMetadataRepository.GetMetadataAsync(
		Id<FileMetadata> fileId,
		CancellationToken cancellationToken
	) {
		string filename = Path.Combine( _fileFolder, fileId.Value + ".metadata" );
		using FileStream fs = new FileStream( filename, FileMode.Open, FileAccess.Read, FileShare.Read );
		FileMetadata? result = await JsonSerializer.DeserializeAsync<FileMetadata>( fs, _options, cancellationToken ).ConfigureAwait( false );
		return result ?? throw new FileNotFoundException();
	}

	async Task IMutableFileMetadataRepository.PutMetadataAsync(
		FileMetadata fileMetadata,
		CancellationToken cancellationToken
	) {
		string filename = Path.Combine( _fileFolder, fileMetadata.FileId.Value + ".metadata" );
		using FileStream fs = new FileStream( filename, FileMode.Create, FileAccess.Write, FileShare.None );
		await JsonSerializer.SerializeAsync( fs, fileMetadata, _options, cancellationToken ).ConfigureAwait( false );
	}
}

