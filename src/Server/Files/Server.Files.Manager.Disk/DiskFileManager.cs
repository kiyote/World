using Common.Files.Manager;

namespace Server.Files.Manager.Disk;

[System.Diagnostics.CodeAnalysis.SuppressMessage( "Performance", "CA1812:An internal (assembly-level) type is never instantiated.", Justification = "This class is instantiated via DI." )]
internal class DiskFileManager : MutableFileManager<DiskFileRepository, DiskFileRepository>, IDiskFileManager {

	public DiskFileManager(
		DiskFileRepository fileContentRepository,
		DiskFileRepository fileMetadataRepository
	) : base(
		fileContentRepository,
		fileMetadataRepository
	) {
	}
}

