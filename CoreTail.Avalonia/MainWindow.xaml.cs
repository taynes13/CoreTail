using System.Collections.Specialized;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace CoreTail.Avalonia
{
    public class MainWindow : Window
    {
        private ListBox _listBox;

        public MainWindow()
        {
            InitializeComponent();
            App.AttachDevTools(this);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            _listBox = this.FindControl<ListBox>("listBox");
            _listBox.DataContextChanged += _listBox_DataContextChanged;

            // TODO: this is workaround, I don't know how set DataContext of a window externally
            DataContext = App.MainWindowDataContext;
        }

        private void _listBox_DataContextChanged(object sender, System.EventArgs e)
        {
            // TODO: unsubscription missing
            var listBoxItems = _listBox.Items as INotifyCollectionChanged;
            if (listBoxItems != null)
                listBoxItems.CollectionChanged += ListBoxItems_CollectionChanged;
        }

        private void ListBoxItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ScrollToBottom();
        }

        private void ScrollToBottom()
        {
            if (_listBox?.Scroll == null)
                return;

            _listBox.Scroll.Offset = new global::Avalonia.Vector(_listBox.Scroll.Offset.X, _listBox.Scroll.Extent.Height);
        }
    }
}
