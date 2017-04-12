using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CoreTail.Shared
{ 
    public class FileReaderViewModel : IDisposable
    {
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        public ObservableCollection<string> LogContent { get; } = new ObservableCollection<string>();

        public FileReaderViewModel(string fileName)
        {
            if (!File.Exists(fileName))
                throw new FileNotFoundException("File does not exist", fileName);

            WatchFileAsync(fileName);
        }

        private async void WatchFileAsync(string fileName)
        {
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
