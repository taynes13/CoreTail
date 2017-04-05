using System;

namespace CoreTail.Shared
{
    public interface IDispatcher
    {
        void InvokeAsync(Action callback);
    }
}
