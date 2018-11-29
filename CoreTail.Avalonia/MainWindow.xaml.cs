using System.Collections;
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
            _listBox.PropertyChanged += _listBox_PropertyChanged;
        }

        private void _listBox_PropertyChanged(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property == ItemsControl.ItemsProperty &&
                _listBox.Items is INotifyPropertyChanged listBoxItemsNotifyPropertyChanged)
            {
                listBoxItemsNotifyPropertyChanged.PropertyChanged -= ListBoxItemsNotifyPropertyChanged_PropertyChanged;
                listBoxItemsNotifyPropertyChanged.PropertyChanged += ListBoxItemsNotifyPropertyChanged_PropertyChanged;
            }
        }

        private void ListBoxItemsNotifyPropertyChanged_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ICollection.Count))
                ScrollToBottom();
        }

        private void ScrollToBottom()
        {
            if (_listBox?.Scroll == null) return;
            _listBox.Scroll.Offset = new Vector(_listBox.Scroll.Offset.X, _listBox.Scroll.Extent.Height);
        }
    }
}
