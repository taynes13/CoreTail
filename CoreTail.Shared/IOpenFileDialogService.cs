using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoreTail.Shared
{
    public interface IOpenFileDialogService
    {
        Task<string[]> ShowAsync(OpenFileDialogSettings settings, object ownerWindow = null);
    }
}
