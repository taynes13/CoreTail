using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using CoreTail.Shared;

namespace CoreTail.Wpf.Shared
{
    internal class OpenFileDialogService : IOpenFileDialogService
    {
        public Task<string[]> ShowAsync(OpenFileDialogSettings settings, object ownerWindow = null)
        {
            var dialog = new OpenFileDialog
            {
                Title = settings.Title,
                InitialDirectory = settings.InitialDirectory,
                FileName = settings.InitialFileName,
                Multiselect = settings.AllowMultiple,
                Filter = ToOpenFileDialogFilter(settings.Filters)
            };

            if (dialog.ShowDialog(ownerWindow as Window) != true)
                return Task.FromResult<string[]>(null);

            var fileNames = dialog.FileNames;
            return Task.FromResult(fileNames);
        }

        private static string ToOpenFileDialogFilter(IReadOnlyCollection<FileDialogFilter> filters)
        {
            if (filters == null)
                return null;

            var fileTypes = filters.Select(ToFileType);
            var dialogFilter = string.Join("|", fileTypes);

            return dialogFilter;
        }

        private static string ToFileType(FileDialogFilter filter)
        {
            var fileMasks = filter.Extensions.Select(i => "*." + i).ToArray();
            var fileTypeLabel = filter.Name + " (" + string.Join(", ", fileMasks) + ")";
            var fileTypeExtensions = string.Join(";", fileMasks);
            var fileType = fileTypeLabel + "|" + fileTypeExtensions;

            return fileType;
        }
    }
}
