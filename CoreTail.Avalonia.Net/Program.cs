using Avalonia;
using Avalonia.Controls;
using Serilog;
using System;

namespace CoreTail.Avalonia.Net
{
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            BuildAvaloniaApp()
                .SetupWithoutStarting()
                .InitializeLogging()
                .Instance
                .Run(App.InitializeAndGetMainWindow(args));
        }

        /// <summary>
        /// This method is needed for IDE previewer infrastructure
        /// </summary>
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect();
    }

    internal static class AppBuilderExentions
    {
        public static T InitializeLogging<T>(this T builder) where T : AppBuilderBase<T>, new()
        {
            var logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Trace()
                .CreateLogger();

            Log.Logger = logger;

            return builder.LogToTrace();
        }
    }
}
