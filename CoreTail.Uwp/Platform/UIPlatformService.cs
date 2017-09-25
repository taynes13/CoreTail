using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using CoreTail.Shared.Other;
using CoreTail.Shared.Platform;

namespace CoreTail.Uwp.Platform
{
    internal class UIPlatformService : IUIPlatformService<FileInfo>
    {
        public async Task<FileInfo[]> ShowOpenFileDialogAsync(OpenFileDialogSettings settings, object ownerWindow = null)
        {
            var fileOpenPicker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.List,
                SuggestedStartLocation = PickerLocationId.ComputerFolder
            };

            if (settings.Filters != null)
            {
                foreach (var extension in settings.Filters.SelectMany(f => f.Extensions))
                {
                    fileOpenPicker.FileTypeFilter.Add(
                        extension == "*" ? "*" : "." + extension);
                }
            }

            if (settings.AllowMultiple)
            {
                var files = await fileOpenPicker.PickMultipleFilesAsync();
                return files.Select(storageFile => new FileInfo(storageFile)).ToArray();
            }
            else
            {
                var storageFile = await fileOpenPicker.PickSingleFileAsync();
                return storageFile == null ? new FileInfo[0] : new[] { new FileInfo(storageFile) };
            }
        }        
    }
}
