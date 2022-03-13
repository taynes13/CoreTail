using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NLipsum.Core;

namespace CoreTail.Test.ManualTests
{
    public class PerformanceTests : IDisposable
    {
        private const int TestDurationInSeconds = 60;
        private const int DelayBetweenLinesInMilliseconds = 0;

#if DEBUG
        private const string Configuration = "Debug";
#else
        private const string Configuration = "Release";
#endif

        private const string WpfAppName = "CoreTail.Wpf";
        private const string AvaloniaNetAppName = "CoreTail.Avalonia.Net";
        private const string AvaloniaNetCoreAppName = "CoreTail.Avalonia.NetCore";

        private string _testFileName;
        private CancellationTokenSource _cts;
        private Task _appendTestFileTask;

        public PerformanceTests() => Initialize();
        public void Dispose() => Cleanup();

        private static string GetExecutablePath(string appName, bool isNetCore = false)
        {
            var extension = isNetCore ? "dll" : "exe";
            var subfolder = isNetCore ? "netcoreapp3.1/" : string.Empty;

            return Path.Combine(
                Environment.CurrentDirectory,
                $"../../../{appName}/bin/{Configuration}/{subfolder}{appName}.{extension}");
        }

        private void Initialize()
        {
            _testFileName = Path.GetTempFileName();            

            _cts = new CancellationTokenSource();

            _appendTestFileTask = Task.Run(AppendTestFileAsync);
        }

        private async Task AppendTestFileAsync()
        {
            var lipsumGenerator = new LipsumGenerator();

            using (var fileWriter = new StreamWriter(_testFileName))
            {
                while (!_cts.IsCancellationRequested)
                {
                    var sentence = lipsumGenerator.GenerateSentences(1, Sentence.Long).First();
                    var toAppend = $"{DateTime.UtcNow:yyyy-MM-dd hh:mm:ss.fff} - {sentence}";
                    await fileWriter.WriteLineAsync(toAppend);
                    await fileWriter.FlushAsync();

                    if (DelayBetweenLinesInMilliseconds != 0)
                        await Task.Delay(DelayBetweenLinesInMilliseconds);
                }
            }
        }

        private void Cleanup()
        {
            _cts.Cancel();
            _appendTestFileTask.Wait();

            if (File.Exists(_testFileName))
                File.Delete(_testFileName);
        }

        public void OpenAppendedFileWPF()
        {
            var wpfProcess = Process.Start(GetExecutablePath(WpfAppName), $"\"{_testFileName}\"");

            Task.Delay(TimeSpan.FromSeconds(TestDurationInSeconds))
                .ContinueWith(t => wpfProcess.CloseMainWindow());

            wpfProcess.WaitForExit();
        }

        public void OpenAppendedFileAvaloniaNet()
        {
            var avaloniaNetProcess = Process.Start(GetExecutablePath(AvaloniaNetAppName), $"\"{_testFileName}\"");

            Task.Delay(TimeSpan.FromSeconds(TestDurationInSeconds))
                .ContinueWith(t => avaloniaNetProcess.CloseMainWindow());
            
            avaloniaNetProcess.WaitForExit();
        }

        public void OpenAppendedFileAvaloniaNetCore()
        {
            var avaloniaNetCoreProcess = Process.Start(
                @"dotnet", 
                $"exec \"{GetExecutablePath(AvaloniaNetCoreAppName, true)}\" \"{_testFileName}\"");

            Task.Delay(TimeSpan.FromSeconds(TestDurationInSeconds))
                .ContinueWith(t => avaloniaNetCoreProcess.CloseMainWindow());

            avaloniaNetCoreProcess.WaitForExit();
        }

        public void OpenAppendedFileAllPlatforms()
        {
            var wpfProcess = Process.Start(GetExecutablePath(WpfAppName), $"\"{_testFileName}\"");
            var avaloniaNetProcess = Process.Start(GetExecutablePath(AvaloniaNetAppName), $"\"{_testFileName}\"");
            var avaloniaNetCoreProcess = Process.Start(
                @"dotnet",
                $"exec \"{GetExecutablePath(AvaloniaNetCoreAppName, true)}\" \"{_testFileName}\"");

            Task.Delay(TimeSpan.FromSeconds(TestDurationInSeconds))
                .ContinueWith(t =>
                {
                    wpfProcess.CloseMainWindow();
                    avaloniaNetProcess.CloseMainWindow();
                    avaloniaNetCoreProcess.CloseMainWindow();
                });

            wpfProcess.WaitForExit();
            avaloniaNetProcess.WaitForExit();
            avaloniaNetCoreProcess.WaitForExit();
        }
    }
}
