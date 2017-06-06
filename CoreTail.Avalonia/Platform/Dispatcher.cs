using System;
using CoreTail.Shared.Platform;

namespace CoreTail.Avalonia.Platform
{
    internal class Dispatcher : IDispatcher
    {
        public void InvokeAsync(Action callback)
        {
            global::Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(callback);
        }
    }
}
