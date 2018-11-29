using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CoreTail.Avalonia.Platform;
using CoreTail.Shared;
using CoreTail.Shared.Platform;

namespace CoreTail.Avalonia
{
    public class App : Application
    {
        public override void Initialize()
        {
            base.Initialize();
            AvaloniaXamlLoader.Load(this);
        }

        public static Window InitializeAndGetMainWindow(string[] args)
        {
            var viewModel = CreateViewModel();

            var mainWindow = new MainWindow { DataContext = viewModel };
            mainWindow.Closed += (o, args2) => viewModel.Dispose(); // invoked, but message loop is not drained before process end, probably Avalonia bug!

            var fileInfo = args.Length == 0 ? null : new FileInfo(args[0]);

            if (fileInfo != null)
                viewModel.OpenAndWatchFileAsync(fileInfo);

            return mainWindow;
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
