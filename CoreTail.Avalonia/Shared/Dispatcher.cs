using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreTail.Avalonia.Shared
{
    internal class Dispatcher : CoreTail.Shared.IDispatcher
    {
        public void Invoke(Action callback)
        {
            global::Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                callback();
            });
        }
    }
}
