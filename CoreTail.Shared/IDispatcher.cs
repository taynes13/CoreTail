using System;
using System.Collections.Generic;
using System.Text;

namespace CoreTail.Shared
{
    public interface IDispatcher
    {
        void Invoke(Action callback);
    }
}
