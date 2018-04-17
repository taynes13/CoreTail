using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using CoreTail.Shared;
using CoreTail.Shared.Platform;
using CoreTail.Test.Helpers;
using FileInfo = CoreTail.Shared.Platform.FileInfo;
using TimeoutException = System.TimeoutException;

namespace CoreTail.Test
{
    [TestClass]
    public class FileReaderViewModelTests
    {
        private const string TestString = "this is test";

        [TestMethod]
        public async Task AppendTest_FromBeginning_Text() => await RunTest(TestString);

        [TestMethod]
        public async Task AppendTest_FromBeginning_NewLine() => await RunTest(Environment.NewLine);

        [TestMethod]
        public async Task AppendTest_FromBeginning_NewLineAndText() => await RunTest(Environment.NewLine + TestString);

        [TestMethod]
        public async Task AppendTest_FromBeginning_TextAndNewLine() => await RunTest(TestString + Environment.NewLine);

        [TestMethod]
        public async Task AppendTest_AfterText_Text() => await RunTest(TestString, TestString);

        [TestMethod]
        public async Task AppendTest_AfterText_NewLine() => await RunTest(TestString, Environment.NewLine);

        [TestMethod]
        public async Task AppendTest_AfterText_NewLineAndText() => await RunTest(TestString, Environment.NewLine + TestString);

        [TestMethod]
        public async Task AppendTest_AfterText_TextAndNewLine() => await RunTest(TestString, TestString + Environment.NewLine);

        [TestMethod]
        public async Task AppendTest_AfterNewLine_Text() => await RunTest(TestString + Environment.NewLine, TestString);

        [TestMethod]
        public async Task AppendTest_AfterNewLine_NewLine() => await RunTest(TestString + Environment.NewLine, Environment.NewLine);

        [TestMethod]
        public async Task AppendTest_AfterNewLine_NewLineAndText() => await RunTest(TestString + Environment.NewLine, Environment.NewLine + TestString);

        [TestMethod]
        public async Task AppendTest_AfterNewLine_TextAndNewLine() => await RunTest(TestString + Environment.NewLine, TestString + Environment.NewLine);

        private static async Task RunTest(params string[] stringsToWrite)
        {
            var linesExpected = Regex.Split(
                string.Join(string.Empty, stringsToWrite), 
                Environment.NewLine, 
                RegexOptions.Compiled);

            if (linesExpected.LastOrDefault() == string.Empty)
                linesExpected = linesExpected.Take(linesExpected.Length - 1).ToArray();

            var vm = await RunTest(async sw =>
            {
                for (var i = 0; i < stringsToWrite.Length; i++)
                {
                    if (i > 0) await Task.Delay(200);
                    
                    await sw.WriteAsync(stringsToWrite[i]);
                    await sw.FlushAsync();
                }
            });

            try
            {
                await vm.LogContent.WaitUntil(col => col.SequenceEqual(linesExpected), TimeSpan.FromSeconds(1));
            }
            catch (TimeoutException)
            {
               Assert.Fail("LogContent does not match - Expected: {0}, Actual: {1}", string.Join("<NewLine>", linesExpected), string.Join("<NewLine>", vm.LogContent));
            }
        }

        private static async Task<FileReaderViewModel<FileInfo>> RunTest(Func<StreamWriter, Task> streamWriterAsyncAction)
        {
            var uiPlatformServiceMock = new Mock<IUIPlatformService<FileInfo>>();
        
            var systemPlatformServiceMock = new Mock<ISystemPlatformService<FileInfo>>();
            Stream fileStreamMock = new ProducerConsumerStream();
            systemPlatformServiceMock
                .Setup(x => x.OpenFileAsStream(It.IsAny<FileInfo>()))
                .ReturnsAsync(fileStreamMock);

            var vm = new FileReaderViewModel<FileInfo>(
                uiPlatformServiceMock.Object, 
                systemPlatformServiceMock.Object);

            await vm.OpenAndWatchFileAsync(new FileInfo("DummyPath"));

            using (var streamWriter = new StreamWriter(fileStreamMock, Encoding.UTF8, 1024, true))
                await streamWriterAsyncAction(streamWriter);

            return vm;
        }
    }
}
