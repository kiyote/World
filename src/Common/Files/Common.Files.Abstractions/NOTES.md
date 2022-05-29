Precondition:
A mutable file system must support immutable file operations.


To build a file system, you implement:
IImmutableFileContentRepository -> IMutableFileContentRepository
IImmutableFileManager -> IImmutableFileContentRepository, IImmutableFileMetadataRepository

and optionally implement:
IImmutableFileMetadataRepository -> IMutableFileMetadataRepository 
(your IMutableFileMetadataRepository must implement IImmutableFileMetadataRepository)
IMutableFileManager -> IImmutableFileManager, IMutableFileContentRepository, IMutableFileMetadataRepository


Then, make a marker interface of I<Foo>FileManager which implements IImmutableFileManager,
and optionally implements IMutableFileManager


Then, clients can ask for I<Foo>FileManager to get the file system they're after.



TODO:
Create a IFileSystems that takes an enumerable of all file systems available.  It then
iterates any operation through them trying them in turn to see which is the first to
respond to the request.