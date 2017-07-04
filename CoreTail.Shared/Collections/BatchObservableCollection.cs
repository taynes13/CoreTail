using CoreTail.Shared.Other;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;

namespace CoreTail.Shared.Collections
{
    // See: https://stackoverflow.com/questions/8606994/adding-a-range-of-values-to-an-observablecollection-efficiently
    public class BatchObservableCollection<T> : System.Collections.ObjectModel.ObservableCollection<T>
    {
        public void AddRange(IEnumerable<T> collection)
        {
            Guard.ArgumentNotNull(collection, nameof(collection));
            CheckReentrancy();

            var startIndex = Count;

            foreach (var item in collection)
                Items.Add(item);

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
        }
    }
}
