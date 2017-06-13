using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CoreTail.Shared.Other;
using CoreTail.Shared.Platform;
using CoreTail.Shared.Collections;

namespace CoreTail.Shared
{
    public class FileReaderViewModel<TFileInfo> : IDisposable where TFileInfo : class, IFileInfo 
    {
        private readonly ISystemPlatformService<TFileInfo> _systemPlatformService;
        private readonly IUIPlatformService<TFileInfo> _uiPlatformService;

        private CancellationTokenSource _cts;

        public BatchObservableCollection<string> LogContent { get; } = new BatchObservableCollection<string>();

        public ICommand FileOpenCommand { get; }

        public FileReaderViewModel(
            IUIPlatformService<TFileInfo> uiPlatformService, 
            ISystemPlatformService<TFileInfo> systemPlatformService)
        {
            _uiPlatformService = Guard.ArgumentNotNull(uiPlatformService, nameof(uiPlatformService));
            _systemPlatformService = Guard.ArgumentNotNull(systemPlatformService, nameof(systemPlatformService));

            FileOpenCommand = new DelegateCommand(async () => await ExecuteFileOpenCommand());
        }

        public async Task OpenAndWatchFileAsync(TFileInfo fileInfo)
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            var cts = _cts;

            LogContent.Clear();

            var stream = await _systemPlatformService.OpenFileAsStream(fileInfo);
            
            // reading of file is not awaited deliberately (only opening of file is awaited)
            WatchFileInternal(stream, cts);
        }

        public void Dispose()
        {
            _cts?.Cancel();
        }

        private async Task WatchFileInternal(Stream stream, CancellationTokenSource cts)
        {
            var lines = new List<string>();
            using (var fileStream = stream)
            using (var fileReader = new StreamReader(fileStream))
            {
                while (!cts.IsCancellationRequested)
                {
                    var line = await fileReader.ReadLineAsync();

                    if (IsEof(line))
                    {
                        if (lines != null)
                        {
                            LogContent.AddRange(lines);
                            lines.Clear();
                        }

                        await Task.Delay(100, cts.Token); // TODO: acceptable delay?
                    }
                    else
                    {
                        lines.Add(line);
                    }
                }
            }
        }

        private static bool IsEof(string line)
        {
            return line == null;
        }

        private async Task ExecuteFileOpenCommand()
        {
            var dialogSettings = new OpenFileDialogSettings
            {
                AllowMultiple = false,
                Filters = new List<FileDialogFilter>
                {
                    new FileDialogFilter { Name = "All Files", Extensions = new List<string> { "*" } }
                }
            };

            var fileInfos = await _uiPlatformService.ShowOpenFileDialogAsync(dialogSettings);
            var fileInfo = fileInfos?.FirstOrDefault();

            if (fileInfo != null)
                await OpenAndWatchFileAsync(fileInfo);
        }
    }
}
