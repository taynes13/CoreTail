using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace CoreTail.Test.Helpers
{
    internal static class ObservableCollectionExtensions
    {
        public static async Task WaitUntil<T>(
            this ObservableCollection<T> collection, 
            Func<ObservableCollection<T>, bool> predicate, 
            TimeSpan timeout = default(TimeSpan))
        {
            var tcs = new TaskCompletionSource<bool>();

            if (predicate(collection))
            {
                tcs.SetResult(true);
                return;
            }

            collection.CollectionChanged += (sender, e) =>
            {
                if (predicate(collection))
                    tcs.SetResult(true);
            };

            IEnumerable<Task> GetInnerTasks()
            {
                yield return tcs.Task;

                if (timeout != default(TimeSpan))
                    yield return Task.Delay(timeout).ContinueWith(t => throw new TimeoutException());
            }

            await await Task.WhenAny(GetInnerTasks());
        }
    }
}
