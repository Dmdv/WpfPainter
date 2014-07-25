using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace WpfPainter.Adorners
{
	internal class FourBoxesAdorner : Adorner
	{
		public FourBoxesAdorner(UIElement adornedElement)
			:
				base(adornedElement)
		{
		}

		protected override void OnRender(DrawingContext context)
		{
			var adornedElementRect = new Rect(AdornedElement.DesiredSize);

			context.DrawRectangle(Brushes.Red,
				null,
				new Rect(adornedElementRect.TopLeft.X, adornedElementRect.TopLeft.Y, 5, 5));
			context.DrawRectangle(Brushes.Red,
				null,
				new Rect(adornedElementRect.BottomLeft.X, adornedElementRect.BottomLeft.Y - 5, 5, 5));
			context.DrawRectangle(Brushes.Red,
				null,
				new Rect(adornedElementRect.TopRight.X - 5, adornedElementRect.TopRight.Y, 5, 5));
			context.DrawRectangle(Brushes.Red,
				null,
				new Rect(adornedElementRect.BottomRight.X - 5, adornedElementRect.BottomRight.Y - 5, 5, 5));
		}
	}
}