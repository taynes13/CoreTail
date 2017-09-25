using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using CoreTail.Shared.Other;

namespace CoreTail.Shared.Collections
{
    // See: https://stackoverflow.com/questions/8606994/adding-a-range-of-values-to-an-observablecollection-efficiently
    public class BatchObservableCollection<T> : ObservableCollection<T>
    {
        public void AddRange(IEnumerable<T> collection)
        {
            Guard.ArgumentNotNull(collection, nameof(collection));
            CheckReentrancy();

            foreach (var item in collection)
                Items.Add(item);

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
            OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
        }
    }
}
