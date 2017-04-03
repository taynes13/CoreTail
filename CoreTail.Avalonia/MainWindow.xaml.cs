using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using CoreTail.Avalonia.Shared;
using CoreTail.Shared;
using System.Collections.Specialized;

namespace CoreTail.Avalonia
{
    public class MainWindow : Window
    {
        private ListBox _listBox;
        private ScrollViewer _scrollViewer;
        private TextBlock _logSizeTextBlock;

        private Dispatcher _dispatcher;
        private MainWindowViewModel _viewModel;

        public MainWindow()
        {
            this.InitializeComponent();
            App.AttachDevTools(this);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            _listBox = this.FindControl<ListBox>("listBox");
            _listBox.TemplateApplied += _listBox_TemplateApplied;
            _logSizeTextBlock = this.FindControl<TextBlock>("logSizeTextBlock");

            _dispatcher = new Dispatcher();
            _viewModel = new MainWindowViewModel(_dispatcher);
            _viewModel.Initialize();

            this.DataContext = _viewModel;
            _viewModel.LogContent.CollectionChanged += LogContent_CollectionChanged;
            _listBox.Items = _viewModel.LogContent;
        }

        private void LogContent_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ScrollToBottom();
        }

        private void _listBox_TemplateApplied(object sender, TemplateAppliedEventArgs e)
        {
            _scrollViewer = e.NameScope.Find<ScrollViewer>("PART_ScrollViewer");
        }

        private void ScrollToBottom()
        {
            if (_listBox == null || _listBox.Scroll == null)
                return;

            _listBox.Scroll.Offset = new global::Avalonia.Vector(_listBox.Scroll.Offset.X, _listBox.Scroll.Extent.Height);
        }
    }
}
