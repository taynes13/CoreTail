using System.ComponentModel;
using Avalonia;
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
            this.AttachDevTools();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            _listBox = this.FindControl<ListBox>("listBox");
            _listBox.DataContextChanged += _listBox_DataContextChanged;
        }

        private void _listBox_DataContextChanged(object sender, System.EventArgs e)
        {
            if (_listBox.Items is INotifyPropertyChanged listBoxItemsNotifyPropertyChanged)
            {
                listBoxItemsNotifyPropertyChanged.PropertyChanged -= ListBoxItemsNotifyPropertyChanged_PropertyChanged;
                listBoxItemsNotifyPropertyChanged.PropertyChanged += ListBoxItemsNotifyPropertyChanged_PropertyChanged;
            }
        }

        private void ListBoxItemsNotifyPropertyChanged_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Count")
                ScrollToBottom();
        }

        private void ScrollToBottom()
        {
            if (_listBox?.Scroll == null) return;
            _listBox.Scroll.Offset = new global::Avalonia.Vector(_listBox.Scroll.Offset.X, _listBox.Scroll.Extent.Height);
        }
    }
}
