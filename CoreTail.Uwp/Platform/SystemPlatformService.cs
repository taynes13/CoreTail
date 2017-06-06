using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using CoreTail.Shared.Platform;

namespace CoreTail.Uwp.Platform
{
    internal class SystemPlatformService : ISystemPlatformService<FileInfo>
    {
        public async Task<Stream> OpenFileAsStream(FileInfo fileInfo)
        {
            return (await fileInfo
                    .StorageFile
                    .OpenAsync(FileAccessMode.Read, StorageOpenOptions.AllowReadersAndWriters)
                    .AsTask())
                .AsStreamForRead();
        }
    }
}
