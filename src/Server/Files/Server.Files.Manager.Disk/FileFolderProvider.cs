namespace Server.Files.Manager.Disk;

internal class FileFolderProvider : IFileFolderProvider {

	private readonly string _fileFolder;

	public FileFolderProvider(
		string fileFolder
	) {
		_fileFolder = fileFolder;
	}

	string IFileFolderProvider.GetLocation() {
		return _fileFolder;
	}
}

