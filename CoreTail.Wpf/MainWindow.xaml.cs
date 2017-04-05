using CoreTail.Shared;
using CoreTail.Wpf.Shared;

namespace CoreTail.Wpf
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            var viewModel = new MainWindowViewModel(new Dispatcher(Dispatcher));
            viewModel.Initialize();

            DataContext = viewModel;
        }
    }
}
