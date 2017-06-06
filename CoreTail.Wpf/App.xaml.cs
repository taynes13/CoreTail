using System.Windows;
using CoreTail.Shared;
using CoreTail.Shared.Platform;
using CoreTail.Wpf.Platform;

namespace CoreTail.Wpf
{
    public partial class App
    {
        private async void App_OnStartup(object sender, StartupEventArgs e)
        {
            var viewModel = CreateViewModel();

            var mainWindow = new MainWindow { DataContext = viewModel };

            mainWindow.Closed += (o, args) => viewModel.Dispose();

            mainWindow.Show();

            var fileInfo = e.Args.Length == 0 ? null : new FileInfo(e.Args[0]);

            if (fileInfo != null)
                await viewModel.OpenAndWatchFileAsync(fileInfo);
        }

        private FileReaderViewModel<FileInfo> CreateViewModel()
        {
            return  new ViewModelFactory<FileInfo>(
                    new Dispatcher(Dispatcher),
                    new UIPlatformService(),
                    new SystemPlatformService())
                .CreateMainWindowViewModel();
        }
    }
}
