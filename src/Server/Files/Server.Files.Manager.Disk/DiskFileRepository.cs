using System.IO.Abstractions;
using System.Text.Json;
using Common.Files.Manager;
using Common.Files.Manager.Repositories;

namespace Server.Files.Manager.Disk;

[System.Diagnostics.CodeAnalysis.SuppressMessage( "Performance", "CA1812:An internal (assembly-level) type is never instantiated.", Justification = "This class is instantiated via DI." )]
internal class DiskFileRepository : IDiskFileRepository {

	private readonly JsonSerializerOptions _options;
	private readonly IFileFolderProvider _fileFolderProvider;
	private readonly IFileSystem _fileSystem;

	public DiskFileRepository(
		IFileFolderProvider fileFolderProvider,
		IFileSystem fileSystem
	) {
		_fileFolderProvider = fileFolderProvider;
		_fileSystem = fileSystem;
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

	async Task<bool> IImmutableFileContentRepository.TryGetContentAsync(
		Id<FileMetadata> fileId,
		Func<Stream, Task> contentReader,
		CancellationToken cancellationToken
	) {
		string filename = Path.Combine( _fileFolderProvider.GetLocation(), fileId.Value );
		if (!_fileSystem.File.Exists( filename )) {
			return false;
		}
		using Stream fs = _fileSystem.FileStream.Create( filename, FileMode.Open, FileAccess.Read, FileShare.Read );
		await contentReader( fs ).ConfigureAwait( false );
		return true;
	}

	async Task<FileMetadata> IImmutableFileMetadataRepository.GetMetadataAsync(
		Id<FileMetadata> fileId,
		CancellationToken cancellationToken
	) {
		string filename = Path.Combine( _fileFolderProvider.GetLocation(), fileId.Value + ".metadata" );
		using Stream fs = _fileSystem.FileStream.Create( filename, FileMode.Open, FileAccess.Read, FileShare.Read );
		FileMetadata? result = await JsonSerializer.DeserializeAsync<FileMetadata>( fs, _options, cancellationToken ).ConfigureAwait( false );
		return result ?? throw new FileNotFoundException();
	}

	async Task IMutableFileContentRepository.PutContentAsync(
		Id<FileMetadata> fileId,
		Func<Stream, Task> asyncWriter,
		CancellationToken cancellationToken
	) {
		string filename = Path.Combine( _fileFolderProvider.GetLocation(), fileId.Value );
		using Stream fs = _fileSystem.FileStream.Create( filename, FileMode.Create, FileAccess.Write, FileShare.None );
		await asyncWriter( fs ).ConfigureAwait( false );
	}

	async Task IMutableFileMetadataRepository.PutMetadataAsync(
		FileMetadata fileMetadata,
		CancellationToken cancellationToken
	) {
		string filename = Path.Combine( _fileFolderProvider.GetLocation(), fileMetadata.FileId.Value + ".metadata" );
		using Stream fs = _fileSystem.FileStream.Create( filename, FileMode.Create, FileAccess.Write, FileShare.None );
		await JsonSerializer.SerializeAsync( fs, fileMetadata, _options, cancellationToken ).ConfigureAwait( false );
	}
}

