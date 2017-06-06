using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace CoreTail.Uwp.Extensions
{
    public static class DependencyObjectExtensions
    {
        // adapted from: https://stackoverflow.com/questions/636383/how-can-i-find-wpf-controls-by-name-or-type
        public static T FindChild<T>(this DependencyObject parent, string childName = null)
            where T : DependencyObject
        {
            if (parent == null) throw new ArgumentNullException(nameof(parent));

            T foundChild = null;

            var childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                T childType = child as T;
                if (childType == null)
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }
    }
}
