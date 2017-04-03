using CoreTail.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreTail.Wpf.Shared
{
    internal class Dispatcher : IDispatcher
    {
        private readonly global::System.Windows.Threading.Dispatcher _wpfDispatcher;

        public Dispatcher(global::System.Windows.Threading.Dispatcher wpfDispatcher)
        {
            _wpfDispatcher = wpfDispatcher;
        }

        public void Invoke(Action callback)
        {
            _wpfDispatcher.Invoke(callback);
        }
    }
}
