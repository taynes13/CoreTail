using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using CoreTail.Wpf.Extensions;

namespace CoreTail.Wpf
{
    public partial class MainWindow
    {
        private ScrollViewer _listBoxScrollViewer;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ListBox_OnLoaded(object sender, RoutedEventArgs e)
        {
            _listBoxScrollViewer = ListBox.FindChild<ScrollViewer>();

            if (ListBox.Items is INotifyCollectionChanged listBoxItems)
            {
                listBoxItems.CollectionChanged -= ListBoxItems_CollectionChanged;
                listBoxItems.CollectionChanged += ListBoxItems_CollectionChanged;
            }
        }

        // TODO: throttle
        private void ListBoxItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _listBoxScrollViewer?.ScrollToBottom();
        }
    }
}
