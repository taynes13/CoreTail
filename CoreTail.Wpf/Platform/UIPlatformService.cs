using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using CoreTail.Shared.Other;
using CoreTail.Shared.Platform;

namespace CoreTail.Wpf.Platform
{
    internal class UIPlatformService : IUIPlatformService<FileInfo>
    {
        public Task<FileInfo[]> ShowOpenFileDialogAsync(OpenFileDialogSettings settings, object ownerWindow = null)
        {
            var dialog = new OpenFileDialog
            {
                Title = settings.Title,
                InitialDirectory = settings.InitialDirectory,
                FileName = settings.InitialFileName,
                Multiselect = settings.AllowMultiple,
                Filter = ToOpenFileDialogFilter(settings.Filters)
            };

            return Task.FromResult(
                dialog.ShowDialog(ownerWindow as Window) != true 
                ? null 
                : dialog.FileNames
                    .Select(fileName => new FileInfo(fileName))
                    .ToArray());
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
