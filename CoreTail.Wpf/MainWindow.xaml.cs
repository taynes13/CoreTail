using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CoreTail.Shared;
using CoreTail.Wpf.Extensions;
using FileInfo = CoreTail.Shared.Platform.FileInfo;

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

        private async void OnFileDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;

            async Task SetFileDragOver(string filePath)
            {
                e.Effects = filePath != null ? DragDropEffects.Copy : DragDropEffects.None;
            }

            await ProcessFile(e, SetFileDragOver);
        }

        private async void OnFileDrop(object sender, DragEventArgs e)
        {
            e.Handled = true;

            async Task OpenFile(string filePath)
            {
                if (filePath != null && DataContext is FileReaderViewModel<FileInfo> vm)
                    await vm.OpenAndWatchFileAsync(new FileInfo(filePath));
            }

            await ProcessFile(e, OpenFile);
        }
       
        private static async Task ProcessFile(DragEventArgs e, Func<string, Task> fileAction)
        {
            string filePath = null;
            
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var filesObj = e.Data.GetData(DataFormats.FileDrop);

                if (filesObj is string[] files &&
                    files.Length == 1) // TODO: support for opening multiple files later
                {
                    filePath = files.First();

                    if (!File.Exists(filePath))
                        filePath = null;
                }
            }

            await fileAction(filePath);
        }
    }
}
