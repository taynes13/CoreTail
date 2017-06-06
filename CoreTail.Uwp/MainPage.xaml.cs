using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using CoreTail.Uwp.Extensions;

namespace CoreTail.Uwp
{
    public sealed partial class MainPage
    {
        private ScrollViewer _listBoxScrollViewer;

        public MainPage()
        {
            InitializeComponent();
        }

        private void ListBox_OnLoaded(object sender, RoutedEventArgs e)
        {
            _listBoxScrollViewer = ListBox.FindChild<ScrollViewer>();

            if (ListBox.Items is IObservableVector<object> listBoxItems)
            {
                listBoxItems.VectorChanged -= ListBoxItems_VectorChanged;
                listBoxItems.VectorChanged += ListBoxItems_VectorChanged;
            }
        }

        // TODO: throttle
        private void ListBoxItems_VectorChanged(IObservableVector<object> sender, IVectorChangedEventArgs @event)
        {
            _listBoxScrollViewer?.ChangeView(0.0f, double.MaxValue, 1.0f);
        }

        private void FileMenuButton_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }
    }
}
