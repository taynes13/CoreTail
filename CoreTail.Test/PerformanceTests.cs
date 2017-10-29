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
    // TODO: move to console project?
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
        private const string AvaloniaNetAppName = "CoreTail.Avalonia.Net";
        private const string AvaloniaNetCoreAppName = "CoreTail.Avalonia.NetCore";
        private const string UwpAppPackageFamilyAndId = "5a3cae1b-7b4c-4ed2-be89-40911aa7f6ae_n6fhnfby9ccnm!App";
        private const string UwpAppName = "CoreTail.Uwp";

        private string _testFileName;
        private CancellationTokenSource _cts;
        private Task _appendTestFileTask;

        private static string GetExecutablePath(string appName, bool isNetCore = false)
        {
            var extension = isNetCore ? "dll" : "exe";
            var subfolder = isNetCore ? "netcoreapp2.0/" : string.Empty;

            return Path.Combine(
                Environment.CurrentDirectory,
                $"../../../{appName}/bin/{Configuration}/{subfolder}{appName}.{extension}");
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
        public void OpenAppendedFileAvaloniaNet()
        {
            var avaloniaNetProcess = Process.Start(GetExecutablePath(AvaloniaNetAppName), $"\"{_testFileName}\"");

            Task.Delay(TimeSpan.FromSeconds(TestDurationInSeconds))
                .ContinueWith(t => avaloniaNetProcess.CloseMainWindow());
            
            avaloniaNetProcess.WaitForExit();
        }

        //[TestMethod]
        public void OpenAppendedFileAvaloniaNetCore()
        {
            var avaloniaNetCoreProcess = Process.Start(
                @"dotnet", 
                $"exec \"{GetExecutablePath(AvaloniaNetCoreAppName, true)}\" \"{_testFileName}\"");

            Task.Delay(TimeSpan.FromSeconds(TestDurationInSeconds))
                .ContinueWith(t => avaloniaNetCoreProcess.CloseMainWindow());

            avaloniaNetCoreProcess.WaitForExit();
        }

        //[TestMethod]
        public void OpenAppendedFileUwp()
        {            
            var uwpProcess = StartUwpApp();

            Task.Delay(TimeSpan.FromSeconds(TestDurationInSeconds))
                .ContinueWith(t => uwpProcess.Kill()); // not sure how to cleanly close UWP app

            uwpProcess.WaitForExit();
        }

        private static Process StartUwpApp()
        {
            try
            {
                // TODO: file must be opened manually
                Process.Start(
                    new ProcessStartInfo
                    {
                        FileName = "shell:appsFolder\\" + UwpAppPackageFamilyAndId,
                        UseShellExecute = true
                    });
            }
            catch (Exception ex)
            {
                // TODO: command line package creation and deployment would be nicer - I know but it is complicated
                throw new Exception("Starting UWP app failed - make sure it is installed (or executed from VS at least once)",
                    ex);
            }

            Process uwpProcess = null;

            for (var i = 0; i < 10 && uwpProcess == null; i++)
            {
                Thread.Sleep(1000);
                uwpProcess = Process.GetProcessesByName(UwpAppName).FirstOrDefault();
            }

            if (uwpProcess == null)
                throw new Exception("Starting UWP app failed");

            return uwpProcess;
        }

        [TestMethod]
        public void OpenAppendedFileAllPlatforms()
        {
            var wpfProcess = Process.Start(GetExecutablePath(WpfAppName), $"\"{_testFileName}\"");
            var avaloniaNetProcess = Process.Start(GetExecutablePath(AvaloniaNetAppName), $"\"{_testFileName}\"");
            var avaloniaNetCoreProcess = Process.Start(
                @"dotnet",
                $"exec \"{GetExecutablePath(AvaloniaNetCoreAppName, true)}\" \"{_testFileName}\"");
            var uwpProcess = StartUwpApp();

            Task.Delay(TimeSpan.FromSeconds(TestDurationInSeconds))
                .ContinueWith(t =>
                {
                    wpfProcess.CloseMainWindow();
                    avaloniaNetProcess.CloseMainWindow();
                    avaloniaNetCoreProcess.CloseMainWindow();
                    uwpProcess.Kill();
                });

            wpfProcess.WaitForExit();
            avaloniaNetProcess.WaitForExit();
            avaloniaNetCoreProcess.WaitForExit();
            uwpProcess.WaitForExit();
        }
    }
}
