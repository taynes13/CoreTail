using System;
using CoreTail.Shared;

namespace CoreTail.Avalonia.Shared
{
    internal class Dispatcher : IDispatcher
    {
        public void InvokeAsync(Action callback)
        {
            global::Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(callback);
        }
    }
}
