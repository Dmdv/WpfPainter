using System.Windows;
using System.Windows.Media;

namespace WpfPainter.Extensions
{
	public static class DependencyObjectExtensions
	{
		public static T FindAncestor<T>(this DependencyObject obj)
			where T : DependencyObject
		{
			var parent = VisualTreeHelper.GetParent(obj);
			while (parent != null && !(parent is T))
			{
				parent = VisualTreeHelper.GetParent(parent);
			}
			return parent as T;
		}

		public static T FindVisualChild<T>(this DependencyObject depObj) where T : DependencyObject
		{
			if (depObj != null)
			{
				for (var i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
				{
					var child = VisualTreeHelper.GetChild(depObj, i);
					if (child != null && child is T)
					{
						return (T) child;
					}

					var childItem = FindVisualChild<T>(child);
					if (childItem != null)
					{
						return childItem;
					}
				}
			}
			return null;
		}
	}
}