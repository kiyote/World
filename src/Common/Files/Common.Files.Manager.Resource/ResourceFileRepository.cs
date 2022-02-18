﻿using System.Globalization;
using System.Reflection;
using Common.Files.Manager.Repositories;

namespace Common.Files.Manager.Resource;

[System.Diagnostics.CodeAnalysis.SuppressMessage( "Performance", "CA1812:An internal (assembly-level) type is never instantiated.", Justification = "This class is instantiated via DI." )]
internal sealed class ResourceFileRepository : IResourceFileRepository {

	private readonly Assembly _assembly;
	private const string ResourceRoot = "Common.Files.Manager.Resource.";

	public ResourceFileRepository() {
		_assembly = Assembly.GetExecutingAssembly();
	}

	Task<Stream> IImmutableFileContentRepository.GetContentAsync(
		Id<FileMetadata> fileId,
		CancellationToken cancellationToken
	) {
		Stream? resource = _assembly.GetManifestResourceStream( ResourceRoot + fileId.Value );
		if( resource is null ) {
			throw new FileNotFoundException();
		}
		return Task.FromResult( resource );
	}

	Task<FileMetadata> IImmutableFileMetadataRepository.GetMetadataAsync(
		Id<FileMetadata> fileId,
		CancellationToken cancellationToken
	) {
		string filename = fileId.Value;
		string resourceName = ResourceRoot + filename;
		ManifestResourceInfo? info = _assembly.GetManifestResourceInfo( resourceName );
		if( info is null ) {
			throw new FileNotFoundException();
		}

		MimeTypes.MimeTypeMap.TryGetMimeType( Path.GetExtension( filename ), out string mimeType );
		Stream? resource = _assembly.GetManifestResourceStream( ResourceRoot + fileId.Value )!;
		return Task.FromResult( new FileMetadata( fileId, filename, mimeType, resource.Length, GetBuildDate( _assembly ) ) );
	}

	private static DateTime GetBuildDate( Assembly assembly ) {
		const string BuildVersionMetadataPrefix = "+build";

		AssemblyInformationalVersionAttribute? attribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
		if( attribute?.InformationalVersion != null ) {
			string? value = attribute.InformationalVersion;
			int index = value.IndexOf( BuildVersionMetadataPrefix, StringComparison.Ordinal );
			if( index > 0 ) {
				value = value[( index + BuildVersionMetadataPrefix.Length )..];
				if( DateTime.TryParseExact( value, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime result ) ) {
					return result;
				}
			}
		}

		return default;
	}
}

