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
        // TODO: this is workaround, I don't know how set DataContext of a window externally
        internal static object MainWindowDataContext { get; private set; }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            base.Initialize();

            MainWindowDataContext = CreateViewModel();
        }

        private static MainWindowViewModel CreateViewModel()
        {
            return new MainWindowViewModel(new Dispatcher());
        }

        private static void Main(string[] args)
        {
            InitializeLogging();
            AppBuilder.Configure<App>()
                .UseWin32()
                .UseDirect2D1()
                .Start<MainWindow>();
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
    }
}
