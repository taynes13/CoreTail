using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Diagnostics;
using Avalonia.Logging.Serilog;
using Avalonia.Markup.Xaml;
using CoreTail.Avalonia.Shared;
using CoreTail.Shared;
using Serilog;

namespace CoreTail.Avalonia
{
    internal class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            base.Initialize();
        }

        private static void Main(string[] args)
        {
            InitializeLogging();

            var appBuilder = AppBuilder.Configure<App>()
                .UseWin32()
                .UseDirect2D1()
                .SetupWithoutStarting();

            var viewModel = CreateViewModel(args);

            var mainWindow = new MainWindow { DataContext = viewModel };

            var disposableViewModel = viewModel as IDisposable;
            if (disposableViewModel != null)
                mainWindow.Closed += (o, args2) => disposableViewModel.Dispose(); // invoked, but message loop is not drained before process end, probably Avalonia bug!

            mainWindow.Show();
            appBuilder.Instance.Run(mainWindow);
        }

        public static void AttachDevTools(Window window)
        {
#if DEBUG
            DevTools.Attach(window);
#endif
        }

        private static void InitializeLogging()
        {
#if DEBUG
            SerilogLogger.Initialize(new LoggerConfiguration()
                .MinimumLevel.Warning()
                .WriteTo.Trace(outputTemplate: "{Area}: {Message}")
                .CreateLogger());
#endif
        }

        private static object CreateViewModel(string[] args)
        {
            return args.Length == 0
                ? (object) new RandomGeneratorViewModel(new Dispatcher())
                : new FileReaderViewModel(args[0]);
        }
    }
}
