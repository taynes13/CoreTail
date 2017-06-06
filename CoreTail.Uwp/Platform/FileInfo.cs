using Windows.Storage;
using CoreTail.Shared.Platform;

namespace CoreTail.Uwp.Platform
{
    public class FileInfo : IFileInfo
    {
        public FileInfo(StorageFile storageFile)
        {
            StorageFile = storageFile;
        }

        public StorageFile StorageFile { get; }
    }
}
