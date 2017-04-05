using System;
using CoreTail.Shared;

namespace CoreTail.Wpf.Shared
{
    internal class Dispatcher : IDispatcher
    {
        private readonly System.Windows.Threading.Dispatcher _wpfDispatcher;

        public Dispatcher(System.Windows.Threading.Dispatcher wpfDispatcher)
        {
            _wpfDispatcher = wpfDispatcher;
        }

        public void InvokeAsync(Action callback)
        {
            _wpfDispatcher.BeginInvoke(callback);
        }
    }
}
