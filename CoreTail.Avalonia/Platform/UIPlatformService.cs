using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using CoreTail.Shared.Other;
using CoreTail.Shared.Platform;
using FileDialogFilter = CoreTail.Shared.Other.FileDialogFilter;

namespace CoreTail.Avalonia.Platform
{
    internal class UIPlatformService : IUIPlatformService<FileInfo>
    {
        private readonly Window _parent;

        public UIPlatformService(Window parent)
        {
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }

        public Task<FileInfo[]> ShowOpenFileDialogAsync(OpenFileDialogSettings settings, object ownerWindow = null)
        {
            return new OpenFileDialog
                {
                    AllowMultiple = settings.AllowMultiple,
                    Filters = ToAvaloniaFilters(settings.Filters),
                    InitialDirectory = settings.InitialDirectory,
                    InitialFileName = settings.InitialFileName,
                    Title = settings.Title
                }
                .ShowAsync(_parent)
                .ContinueWith(t => 
                    t.Result?.Select(fileName => new FileInfo(fileName)).ToArray());
        }

        private static List<global::Avalonia.Controls.FileDialogFilter> ToAvaloniaFilters(IReadOnlyCollection<FileDialogFilter> filters)
        {
            if (filters == null)
                return new List<global::Avalonia.Controls.FileDialogFilter>();

            return filters
                .Select(i => new global::Avalonia.Controls.FileDialogFilter
                {
                    Extensions = i.Extensions,
                    Name = i.Name
                })
                .ToList();
        }
    }
}
