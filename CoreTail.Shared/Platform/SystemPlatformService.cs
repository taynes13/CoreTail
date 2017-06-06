using System.IO;
using System.Threading.Tasks;

namespace CoreTail.Shared.Platform
{
    // used in WPF and Avalonia (not in UWP)
    public class SystemPlatformService : ISystemPlatformService<FileInfo>
    {
        public Task<Stream> OpenFileAsStream(FileInfo fileInfo)
        {
            // TODO: test different buffer size - compare performance
            // this itself is sync. operation called from UI thread (should be ok, but not 100% correct)
            return Task.FromResult<Stream>(
                new FileStream(fileInfo.Path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 4096, true));
        }
    }
}
