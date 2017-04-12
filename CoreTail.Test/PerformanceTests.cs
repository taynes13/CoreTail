using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLipsum.Core;

namespace CoreTail.Test
{
    [TestClass]
    public class PerformanceTests
    {
        private const int TestDurationInSeconds = 60;
        private const int DelayBetweenLinesInMilliseconds = 0;

#if DEBUG
        private const string Configuration = "Debug";
#else
        private const string Configuration = "Release";
#endif      
        private const string WpfAppName = "CoreTail.Wpf";
        private const string AvaloniaAppName = "CoreTail.Avalonia";
       
        private string _testFileName;
        private CancellationTokenSource _cts;
        private Task _appendTestFileTask;

        private static string GetExecutablePath(string appName)
        {
            return Path.Combine(
                Environment.CurrentDirectory,
                $"../../../{appName}/bin/{Configuration}/{appName}.exe");
        }

        [TestInitialize]
        public void Initialize()
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

        [TestCleanup]
        public void Cleanup()
        {
            _cts.Cancel();
            _appendTestFileTask.Wait();

            if (File.Exists(_testFileName))
                File.Delete(_testFileName);
        }

        //[TestMethod]
        public void OpenAppendedFileWPF()
        {
            var wpfProcess = Process.Start(GetExecutablePath(WpfAppName), $"\"{_testFileName}\"");

            Task.Delay(TimeSpan.FromSeconds(TestDurationInSeconds))
                .ContinueWith(t => wpfProcess.CloseMainWindow());

            wpfProcess.WaitForExit();
        }

        //[TestMethod]
        public void OpenAppendedFileAvalonia()
        {
            var avaloniaProcess = Process.Start(GetExecutablePath(AvaloniaAppName), $"\"{_testFileName}\"");

            Task.Delay(TimeSpan.FromSeconds(TestDurationInSeconds))
                .ContinueWith(t => avaloniaProcess.CloseMainWindow());

            avaloniaProcess.WaitForExit();
        }

        [TestMethod]
        public void OpenAppendedFileAllPlatforms()
        {
            var wpfProcess = Process.Start(GetExecutablePath(WpfAppName), $"\"{_testFileName}\"");
            var avaloniaProcess = Process.Start(GetExecutablePath(AvaloniaAppName), $"\"{_testFileName}\"");

            Task.Delay(TimeSpan.FromSeconds(TestDurationInSeconds))
                .ContinueWith(t =>
                {
                    wpfProcess.CloseMainWindow();
                    avaloniaProcess.CloseMainWindow();
                });

            wpfProcess.WaitForExit();
            avaloniaProcess.WaitForExit();           
        }
    }
}
