using CoreTail.Shared;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CoreTail.Wpf.Shared
{
    internal class OpenFileDialogService : IOpenFileDialogService
    {
        public Task<string[]> ShowAsync(OpenFileDialogSettings settings, object ownerWindow = null)
        {
            var dialog = new OpenFileDialog();
            dialog.Title = settings.Title;
            dialog.InitialDirectory = settings.InitialDirectory;
            dialog.FileName = settings.InitialFileName;
            dialog.Multiselect = settings.AllowMultiple;
            dialog.Filter = ToOpenFileDialogFilter(settings.Filters);

            if (dialog.ShowDialog(ownerWindow as Window) != true)
            {
                return Task.FromResult<string[]>(null);
            }

            var fileNames = dialog.FileNames;
            return Task.FromResult(fileNames);
        }

        private static string ToFileType(FileDialogFilter filter)
        {
            var fileMasks = filter.Extensions.Select(i => "*." + i).ToArray();
            var fileTypeLabel = filter.Name + " (" + string.Join(", ", fileMasks) + ")";
            var fileTypeExtensions = string.Join(";", fileMasks);
            var fileType = fileTypeLabel + "|" + fileTypeExtensions;

            return fileType;
        }

        private static string ToOpenFileDialogFilter(List<FileDialogFilter> filters)
        {
            if (filters == null)
                return null;

            var fileTypes = filters.Select(ToFileType);
            var dialogFilter = string.Join("|", fileTypes);

            return dialogFilter;
        }
    }
}
