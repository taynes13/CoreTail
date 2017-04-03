using CoreTail.Shared;
using CoreTail.Wpf.Shared;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CoreTail.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Lazy<ScrollViewer> _scrollViewer;

        private IDispatcher _dispatcher;
        private MainWindowViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            _scrollViewer = new Lazy<ScrollViewer>(() =>
            {
                if (VisualTreeHelper.GetChildrenCount(_logContentListBox) > 0)
                {
                    var border = (Border)VisualTreeHelper.GetChild(_logContentListBox, 0);
                    var scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
                    return scrollViewer;
                }
                return null;
            });

            _dispatcher = new Dispatcher(this.Dispatcher);
            _viewModel = new MainWindowViewModel(_dispatcher);
            _viewModel.Initialize();

            this.DataContext = _viewModel;
            _viewModel.LogContent.CollectionChanged += LogContent_CollectionChanged;
        }

        private void LogContent_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ScrollToBottom();
        }

        private void ScrollToBottom()
        {
            _scrollViewer.Value.ScrollToBottom();
        }
    }
}
