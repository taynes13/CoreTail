using System;
using System.Collections.Generic;
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
            // TODO: handle exceptions properly
            WatchFileInternal(stream, cts);
        }

        public void Dispose() => _cts?.Cancel();

        private async Task WatchFileInternal(Stream stream, CancellationTokenSource cts)
        {
            var lines = new List<string>();

            using (var fileStream = stream)
            using (var fileReader = new StreamReader(fileStream))
            {
                var lastLineHadMissingNewLine = false;
                var lastByteReadBuffer = new byte[1];

                while (!cts.IsCancellationRequested)
                {
                    var line = await fileReader.ReadLineAsync();

                    if (IsEof(line))
                    {
                        if (lines.Count > 0)
                        {
                            LogContent.AddRange(lines);
                            lines.Clear();
                        }

                        // TODO: really not possible without delay?
                        await Task.Delay(100); // TODO: acceptable delay?
                    }
                    else
                    {
                        if (line == string.Empty && lastLineHadMissingNewLine)
                        {
                            lastLineHadMissingNewLine = false;
                            continue;
                        }

                        // TODO: measure performance cost
                        // TODO: optimize by reducing seek (only when EoF?)
                        fileReader.BaseStream.Seek(-1, SeekOrigin.Current);

                        lastLineHadMissingNewLine = 
                            fileReader.BaseStream.Read(lastByteReadBuffer, 0, 1) == 1 &&
                            lastByteReadBuffer[0] != '\n' && 
                            lastByteReadBuffer[0] != '\r';

                        // TODO: what if never reaches end of files (producing is faster than consuming?)
                        lines.Add(line);
                    }
                }
            }
        }

        private static bool IsEof(string line) => line == null;

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
