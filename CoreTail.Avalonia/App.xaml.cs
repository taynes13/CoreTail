using Avalonia;
using Avalonia.Controls;
using Avalonia.Diagnostics;
using Avalonia.Logging.Serilog;
using Avalonia.Markup.Xaml;
using CoreTail.Avalonia.Platform;
using Serilog;
using CoreTail.Shared;
using CoreTail.Shared.Platform;

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

            var viewModel = CreateViewModel();

            var mainWindow = new MainWindow { DataContext = viewModel };

            mainWindow.Closed += (o, args2) => viewModel.Dispose(); // invoked, but message loop is not drained before process end, probably Avalonia bug!

            mainWindow.Show();

            var fileInfo = args.Length == 0 ? null : new FileInfo(args[0]);

            if (fileInfo != null)
                viewModel.OpenAndWatchFileAsync(fileInfo);

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

        private static FileReaderViewModel<FileInfo> CreateViewModel()
        {
            return new ViewModelFactory<FileInfo>(
                    new Dispatcher(),
                    new UIPlatformService(),
                    new SystemPlatformService())
                .CreateMainWindowViewModel();
        }
    }
}
