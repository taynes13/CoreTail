using System.Threading.Tasks;
using CoreTail.Shared.Other;

namespace CoreTail.Shared.Platform
{
    public interface IUIPlatformService<TFileInfo> where TFileInfo : IFileInfo
    {
        Task<TFileInfo[]> ShowOpenFileDialogAsync(OpenFileDialogSettings settings, object ownerWindow = null);
    }
}
