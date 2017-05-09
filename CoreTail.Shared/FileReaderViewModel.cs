using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CoreTail.Shared
{ 
    public class FileReaderViewModel : IDisposable
    {
        private IOpenFileDialogService _openFileDialogService;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        public ObservableCollection<string> LogContent { get; } = new ObservableCollection<string>();

        public FileReaderViewModel(IOpenFileDialogService openFileDialogService, string fileName)
        {
            _openFileDialogService = Guard.ArgumentNotNull(openFileDialogService, nameof(openFileDialogService));

            FileOpenCommand = new DelegateCommand(async (arg) => await FileOpenCommandExecute());

            if (!string.IsNullOrEmpty(fileName))
                WatchFileAsync(fileName);
        }

        public ICommand FileOpenCommand
        {
            get; private set;
        }

        private async Task FileOpenCommandExecute()
        {
            var dialogSettings = new OpenFileDialogSettings()
            {
                AllowMultiple = false,
                Filters = new List<FileDialogFilter>()
                {
                    new FileDialogFilter() { Name = "All Files", Extensions = new List<string>() { "*" } }
                }
            };

            var fileNames = await _openFileDialogService.ShowAsync(dialogSettings);
            var fileName = fileNames?.FirstOrDefault();

            if (!string.IsNullOrEmpty(fileName))
                WatchFileAsync(fileName);
        }

        private async void WatchFileAsync(string fileName)
        {
            if (!File.Exists(fileName))
                throw new FileNotFoundException("File does not exist", fileName);

            // TODO: test isAsync=false - compare performance
            // TODO: test different buffer size - compare performance
            using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 4096, true))
            using (var fileReader = new StreamReader(fileStream))
            {
                while (!_cts.IsCancellationRequested)
                {
                    var line = await fileReader.ReadLineAsync();

                    if (line == null) // EOF
                        await Task.Delay(100); // TODO: acceptable delay?
                    else
                        LogContent.Add(line);
                }
            }
        }

        public void Dispose()
        {
            _cts.Cancel();
        }
    }
}
