using System;

namespace CoreTail.Shared.Platform
{
    // TODO: merge into IUIPlatformService?
    public interface IDispatcher
    {
        // TODO: try to avoid using dispatcher, in case it is needed consider changing this method to async
        void InvokeAsync(Action callback);
    }
}
