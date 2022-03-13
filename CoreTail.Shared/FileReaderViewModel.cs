using CoreTail.Shared.Collections;
using CoreTail.Shared.Other;
using CoreTail.Shared.Platform;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Serilog;

namespace CoreTail.Shared
{
    public class FileReaderViewModel<TFileInfo> : IDisposable where TFileInfo : class, IFileInfo
    {
        private readonly ILogger _logger = Log.ForContext<FileReaderViewModel<TFileInfo>>();
        private readonly bool _debugEnabled = Log.IsEnabled(Serilog.Events.LogEventLevel.Debug);

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

        public void Dispose() => _cts?.Cancel();

        private async Task WatchFileInternal(Stream stream, CancellationTokenSource cts)
        {
            var eolIndex = 0;
            var readBuffer = new char[8 * 1024];
            var lineBuilder = new StringBuilder();
            var lineBuilderDirty = false;
            var lines = new List<string>();
            var logContentDirty = false;
            var debugLogPrevCount = 0;
            const int DebugLogFrequency = 1_000;

            using (var fileStream = stream)
            using (var fileReader = new StreamReader(fileStream))
            {
                while (!cts.IsCancellationRequested)
                {
                    var readCount = await fileReader.ReadAsync(readBuffer, 0, readBuffer.Length);

                    if (IsEof(readCount))
                    {
                        if (lines.Count > 0)
                        {
                            if (logContentDirty)
                            {
                                LogContent[LogContent.Count - 1] = lines[0];
                                LogContent.AddRange(lines.Skip(1));
                                logContentDirty = false;
                            }
                            else
                            {
                                LogContent.AddRange(lines);
                            }
                            lines.Clear();
                        }
                        if (lineBuilder.Length > 0 && lineBuilderDirty)
                        {
                            if (logContentDirty)
                            {
                                LogContent[LogContent.Count - 1] = lineBuilder.ToString();
                            }
                            else
                            {
                                LogContent.Add(lineBuilder.ToString());
                            }
                            logContentDirty = true;
                            lineBuilderDirty = false;
                        }
                        if (_debugEnabled)
                        {
                            if (LogContent.Count / DebugLogFrequency > debugLogPrevCount)
                            {
                                _logger.Debug("Logged entries: {LogContentCount}", LogContent.Count);
                                debugLogPrevCount = LogContent.Count / DebugLogFrequency;
                            }
                        }

                        await Task.Delay(100, cts.Token);
                    }
                    else
                    {
                        var readLineStart = 0;
                        for (var i = 0; i < readCount; i++)
                        {
                            if (readBuffer[i] == Environment.NewLine[eolIndex])
                            {
                                eolIndex++;
                            }

                            if (eolIndex == Environment.NewLine.Length)
                            {
                                string line = null;
                                if (lineBuilder.Length > 0)
                                {
                                    if (i > eolIndex)
                                    {
                                        lineBuilder.Append(readBuffer, readLineStart, i - readLineStart - eolIndex + 1);
                                        lineBuilderDirty = true;
                                    }

                                    line = lineBuilder.ToString();
                                    lineBuilder.Clear();
                                    lineBuilderDirty = false;
                                }
                                else
                                {
                                    line = new string(readBuffer, readLineStart, i - readLineStart - eolIndex + 1);
                                }

                                readLineStart = i + 1;
                                eolIndex = 0;
                                lines.Add(line);
                            }
                        }

                        if (readLineStart + eolIndex < readCount)
                        {
                            lineBuilder.Append(readBuffer, readLineStart, readCount - readLineStart - eolIndex);
                            lineBuilderDirty = true;
                        }
                    }
                }
            }
        }

        private static bool IsEof(int readCount) => readCount == 0;

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
