using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfPainter.Extensions;

namespace WpfPainter.Extenders
{
	public class DragHelper : DependencyObject
	{
		public static readonly DependencyProperty CanDragProperty =
			DependencyProperty.RegisterAttached(
				"CanDrag",
				typeof (bool),
				typeof (DragHelper),
				new UIPropertyMetadata(false, OnChangeCanDragProperty));

		// The offset from the top, left of the item being dragged 
		// and the original mouse down
		private static Point _offset;
		private static bool _isDragging;

		public static bool GetCanDrag(UIElement element)
		{
			return (bool) element.GetValue(CanDragProperty);
		}

		public static void SetCanDrag(UIElement element, bool obj)
		{
			element.SetValue(CanDragProperty, obj);
		}

		private static void OnChangeCanDragProperty(
			DependencyObject dependencyObject,
			DependencyPropertyChangedEventArgs eventArgs)
		{
			var element = dependencyObject as UIElement;
			if (element == null)
			{
				return;
			}

			if (eventArgs.NewValue != eventArgs.OldValue)
			{
				if ((bool) eventArgs.NewValue)
				{
					element.PreviewMouseDown += OnPreviewMouseDown;
					element.PreviewMouseUp += OnPreviewMouseUp;
					element.PreviewMouseMove += OnPreviewMouseMove;
				}
				else
				{
					element.PreviewMouseDown -= OnPreviewMouseDown;
					element.PreviewMouseUp -= OnPreviewMouseUp;
					element.PreviewMouseMove -= OnPreviewMouseMove;
				}
			}
		}

		private static void OnPreviewMouseDown(
			object sender,
			MouseButtonEventArgs e)
		{
			// Ensure it's a framework element as we'll need to 
			// get access to the visual tree
			var element = sender as FrameworkElement;
			if (element == null)
			{
				return;
			}

			if (GetCanDrag(element))
			{
				_isDragging = true;
			}

			_offset = e.GetPosition(element);
		}

		private static void OnPreviewMouseMove(
			object sender,
			MouseEventArgs e)
		{
			var element = sender as FrameworkElement;
			if (element == null)
			{
				return;
			}

			if (!_isDragging)
			{
				return;
			}

			var canvas = element.FindAncestor<Canvas>();
			if (canvas == null)
			{
				Debug.WriteLine("item.Parent in ItemsControl is null");
				return;
			}

			// Get the position of the mouse relative to the canvas
			var mousePoint = e.GetPosition(canvas);

			// Offset the mouse position by the original offset position
			mousePoint.Offset(-_offset.X, -_offset.Y);

			// Move the element on the canvas
			element.SetValue(Canvas.LeftProperty, mousePoint.X);
			element.SetValue(Canvas.TopProperty, mousePoint.Y);

			element.CaptureMouse();
		}

		// this is triggered when the mouse is released
		private static void OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
		{
			var element = sender as FrameworkElement;
			if (element == null)
			{
				return;
			}

			_isDragging = false;

			element.ReleaseMouseCapture();
		}
	}
}