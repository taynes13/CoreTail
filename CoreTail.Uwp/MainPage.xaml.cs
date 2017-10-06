using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using CoreTail.Shared;
using CoreTail.Uwp.Extensions;
using CoreTail.Uwp.Platform;

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

        private async void OnFileDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;

            async Task SetFileDragOver(StorageFile storageFile)
            {
                e.AcceptedOperation = DataPackageOperation.Copy;

                if (e.DragUIOverride != null)
                {
                    e.DragUIOverride.Caption = $"Open {storageFile.Name}";
                    e.DragUIOverride.IsContentVisible = true;
                }
            }
            
            await ProcessStorageFile(e, SetFileDragOver);
        }

        private async void OnFileDrop(object sender, DragEventArgs e)
        {
            e.Handled = true;

            async Task OpenFile(StorageFile storageFile)
            {
                if (DataContext is FileReaderViewModel<FileInfo> vm)
                    await vm.OpenAndWatchFileAsync(new FileInfo(storageFile));
            }

            await ProcessStorageFile(e, OpenFile);
        }

        private static async Task ProcessStorageFile(
            DragEventArgs e, 
            Func<StorageFile, Task> storageFileAction)
        {
            if (!e.DataView.Contains(StandardDataFormats.StorageItems)) return;

            var deferral = e.GetDeferral();

            var storageItems = await e.DataView.GetStorageItemsAsync();

            if (storageItems.Count == 1 &&  // TODO: support for opening multiple files later
                storageItems.First() is StorageFile storageFile)
            {
                await storageFileAction(storageFile);
            }

            deferral.Complete();
        }
    }
}
