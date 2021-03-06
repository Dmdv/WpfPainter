using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace WpfPainter.Adorners
{
	// ������������� ����� ������ ��������� ������������ ����� ��������� � �������� ��������.
	// ��� ����� ���� ��������� ��������� � child.
	public class ControlAdorner : Adorner
	{
		public ControlAdorner(UIElement adornedElement)
			: base(adornedElement)
		{
		}

		public FrameworkElement Child
		{
			get { return _child; }
			set
			{
				if (_child != null)
				{
					RemoveVisualChild(_child);
				}
				_child = value;
				if (_child != null)
				{
					AddVisualChild(_child);
				}
			}
		}

		protected override int VisualChildrenCount
		{
			get { return 1; }
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			_child.Arrange(new Rect(new Point(0, 0), finalSize));
			return new Size(_child.ActualWidth, _child.ActualHeight);
		}

		protected override Visual GetVisualChild(int index)
		{
			if (index != 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			return _child;
		}

		protected override Size MeasureOverride(Size constraint)
		{
			_child.Measure(constraint);
			return _child.DesiredSize;
		}

		private FrameworkElement _child;
	}
}