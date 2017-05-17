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
        private readonly IOpenFileDialogService _openFileDialogService;

        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        public ObservableCollection<string> LogContent { get; } = new ObservableCollection<string>();

        public ICommand FileOpenCommand { get; }

        public FileReaderViewModel(IOpenFileDialogService openFileDialogService, string fileName = null)
        {
            _openFileDialogService = Guard.ArgumentNotNull(openFileDialogService, nameof(openFileDialogService));
            
            FileOpenCommand = new DelegateCommand(async () => await ExecuteFileOpenCommand());

            if (!string.IsNullOrEmpty(fileName))
                OpenAndWatchFileAsync(fileName);
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

            var fileNames = await _openFileDialogService.ShowAsync(dialogSettings);
            var fileName = fileNames?.FirstOrDefault();

            if (!string.IsNullOrEmpty(fileName))
                OpenAndWatchFileAsync(fileName);
        }

        private async void OpenAndWatchFileAsync(string fileName)
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
