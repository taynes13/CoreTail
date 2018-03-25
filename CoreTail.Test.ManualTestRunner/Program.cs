using CoreTail.Test.ManualTests;

namespace CoreTail.Test.ManualTestRunner
{
    internal class Program
    {
        private static void Main()
        {
            using (var performanceTests = new PerformanceTests())
            {
                //performanceTests.OpenAppendedFileWPF();
                //performanceTests.OpenAppendedFileAvaloniaNet();
                //performanceTests.OpenAppendedFileAvaloniaNetCore();
                //performanceTests.OpenAppendedFileUwp();
                performanceTests.OpenAppendedFileAllPlatforms();
            }
        }
    }
}
