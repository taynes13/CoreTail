using System.Windows;
using CoreTail.Shared;
using CoreTail.Wpf.Shared;

namespace CoreTail.Wpf
{
    public partial class App
    {
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            var viewModel = CreateViewModel();

            var mainWindow = new MainWindow { DataContext = viewModel };
            mainWindow.Show();
        }

        private MainWindowViewModel CreateViewModel()
        {
            return new MainWindowViewModel(new Dispatcher(Dispatcher));
        }
    }
}
