using Avalonia;
using Avalonia.Logging.Serilog;
using Serilog;
using System;

namespace CoreTail.Avalonia.Net
{
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            InitializeLogging();

            BuildAvaloniaApp().SetupWithoutStarting()
                .Instance.Run(App.InitializeAndGetMainWindow(args));
        }

        /// <summary>
        /// This method is needed for IDE previewer infrastructure
        /// </summary>
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>().UsePlatformDetect();

        // TODO: remove when in Avalonia made into a runtime configuration extension
        private static void InitializeLogging()
        {
#if DEBUG
            SerilogLogger.Initialize(new LoggerConfiguration()
                .MinimumLevel.Warning()
                .WriteTo.Trace(outputTemplate: "{Area}: {Message}")
                .CreateLogger());
#endif
        }
    }
}
