using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using CoreTail.Shared;
using CoreTail.Shared.Platform;
using System.Collections;
using System.ComponentModel;
using System.Linq;

namespace CoreTail.Avalonia
{
    public class MainWindow : Window
    {
        private ListBox _listBox;

        private FileReaderViewModel<FileInfo> ViewModel => DataContext as FileReaderViewModel<FileInfo>;

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

            AddHandler(DragDrop.DropEvent, Drop);
            AddHandler(DragDrop.DragOverEvent, DragOver);
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

        private bool IsDropFilePath(DragEventArgs e)
        {
            return TryGetDropFilePath(e, out _);
        }

        private bool TryGetDropFilePath(DragEventArgs e, out string filePath)
        {
            if (e.Data.Contains(DataFormats.Text) && System.IO.File.Exists(e.Data.GetText()))
            {
                filePath = e.Data.GetText();
                return true;
            }
            else if (e.Data.Contains(DataFormats.FileNames))
            {
                var fileNames = e.Data.GetFileNames().ToList();
                if (fileNames.Count == 1 && System.IO.File.Exists(fileNames[0])) // TODO: Add support for multiple files
                {
                    filePath = fileNames[0];
                    return true;
                }
            }
            filePath = null;
            return false;
        }

        private void DragOver(object sender, DragEventArgs e)
        {
            e.DragEffects = IsDropFilePath(e) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private async void Drop(object sender, DragEventArgs e)
        {
            if (TryGetDropFilePath(e, out var filePath))
            {
                await ViewModel?.OpenAndWatchFileAsync(new FileInfo(filePath));
            }
        }
    }
}
