using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using CoreTail.Shared;
using CoreTail.Shared.Platform;
using CoreTail.Test.Helpers;
using FileInfo = CoreTail.Shared.Platform.FileInfo;

namespace CoreTail.Test
{
    [TestClass]
    public class FileReaderViewModelTests
    {
        [TestMethod]
        public async Task AppendOfOneLineAndNewLineSeparatelyTest()
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

            const string testString = "this is test";

            using (var streamWriter = new StreamWriter(fileStreamMock, Encoding.UTF8, 1024, true))
            {
                streamWriter.Write(testString);
                streamWriter.Flush();
                await Task.Delay(200);
                streamWriter.WriteLine();
                streamWriter.Write(testString);
                streamWriter.Flush();
            }

            await vm.LogContent.WaitUntil(col => col.Count(l => l == testString) == 2, TimeSpan.FromSeconds(1));

            CollectionAssert.AreEqual(new[] { testString, testString}, vm.LogContent);
        }
    }
}
