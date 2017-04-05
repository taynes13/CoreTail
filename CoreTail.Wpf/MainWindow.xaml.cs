using System;
using System.Collections.Specialized;
using System.Windows.Controls;
using System.Windows.Media;
using CoreTail.Shared;
using CoreTail.Wpf.Shared;

namespace CoreTail.Wpf
{
    public partial class MainWindow
    {
        private readonly Lazy<ScrollViewer> _scrollViewer;

        private IDispatcher _dispatcher;
        private MainWindowViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            _scrollViewer = new Lazy<ScrollViewer>(() =>
            {
                if (VisualTreeHelper.GetChildrenCount(LogContentListBox) > 0)
                {
                    var border = (Border)VisualTreeHelper.GetChild(LogContentListBox, 0);
                    var scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
                    return scrollViewer;
                }
                return null;
            });

            _dispatcher = new Dispatcher(Dispatcher);
            _viewModel = new MainWindowViewModel(_dispatcher);
            _viewModel.Initialize();

            DataContext = _viewModel;
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
