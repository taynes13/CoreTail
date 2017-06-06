using CoreTail.Shared.Other;
using CoreTail.Shared.Platform;

namespace CoreTail.Shared
{
    // TODO: required?
    public class ViewModelFactory<TFileInfo> where TFileInfo : class, IFileInfo
    {
        private readonly IDispatcher _dispatcher;
        private readonly IUIPlatformService<TFileInfo> _uiPlatformService;
        private readonly ISystemPlatformService<TFileInfo> _systemPlatformService;

        public ViewModelFactory(
            IDispatcher dispatcher, 
            IUIPlatformService<TFileInfo> uiPlatformService, 
            ISystemPlatformService<TFileInfo> systemPlatformService)
        {
            _dispatcher = Guard.ArgumentNotNull(dispatcher, nameof(dispatcher));
            _uiPlatformService = Guard.ArgumentNotNull(uiPlatformService, nameof(uiPlatformService));
            _systemPlatformService = Guard.ArgumentNotNull(systemPlatformService, nameof(systemPlatformService));
        }

        public FileReaderViewModel<TFileInfo> CreateMainWindowViewModel()
        {
            return new FileReaderViewModel<TFileInfo>(
                _uiPlatformService, 
                _systemPlatformService);
        }
    }
}
