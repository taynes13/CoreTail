using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
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
            {
                ScrollToBottom();
            }
        }

        private void ListBoxItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ScrollToBottom();
        }

        private void ScrollToBottom()
        {
            _listBox.ScrollIntoView(_listBox.Items.Cast<string>().LastOrDefault());
        }
    }
}
