using System.IO;
using System.Threading.Tasks;

namespace CoreTail.Shared.Platform
{
    public interface ISystemPlatformService<in TFileInfo> where TFileInfo : IFileInfo
    {
        Task<Stream> OpenFileAsStream(TFileInfo fileInfo);
    }
}
