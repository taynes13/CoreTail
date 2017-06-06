using System;
using Windows.UI.Core;
using CoreTail.Shared.Platform;

namespace CoreTail.Uwp.Platform
{
    internal class Dispatcher : IDispatcher
    {
        private readonly CoreDispatcher _uwpDispatcher;

        public Dispatcher(CoreDispatcher uwpDispatcher)
        {
            _uwpDispatcher = uwpDispatcher;
        }

        public void InvokeAsync(Action callback)
        {
            // TODO: consider changing this method to async
            _uwpDispatcher.RunAsync(CoreDispatcherPriority.Normal, () => callback()); 
        }
    }
}
