using System;
using System.Windows;
using CoreTail.Shared;
using CoreTail.Wpf.Shared;

namespace CoreTail.Wpf
{
    public partial class App
    {
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            var viewModel = CreateViewModel(e.Args);

            var mainWindow = new MainWindow { DataContext = viewModel };

            var disposableViewModel = viewModel as IDisposable;
            if (disposableViewModel != null)
                mainWindow.Closed += (o, args) => disposableViewModel.Dispose();

            mainWindow.Show();
        }

        private object CreateViewModel(string[] args)
        {
            return new ViewModelFactory(
                    new Dispatcher(Dispatcher),
                    new OpenFileDialogService()
                )
                .CreateMainWindowViewModel(args);
        }
    }
}
