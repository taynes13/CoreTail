using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using CoreTail.Shared;
using FileDialogFilter = CoreTail.Shared.FileDialogFilter;

namespace CoreTail.Avalonia.Shared
{
    internal class OpenFileDialogService : IOpenFileDialogService
    {
        public Task<string[]> ShowAsync(OpenFileDialogSettings settings, object ownerWindow = null)
        {
            return new OpenFileDialog
                {
                    AllowMultiple = settings.AllowMultiple,
                    Filters = ToAvaloniaFilters(settings.Filters),
                    InitialDirectory = settings.InitialDirectory,
                    InitialFileName = settings.InitialFileName,
                    Title = settings.Title
                }
                .ShowAsync(ownerWindow as Window);
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
