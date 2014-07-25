using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace WpfPainter.Adorners
{
	public class SimpleCircleAdorner : Adorner
	{
		// Be sure to call the base class constructor.
		public SimpleCircleAdorner(UIElement adornedElement)
			: base(adornedElement)
		{
		}

		// A common way to implement an adorner's rendering behavior is to override the OnRender
		// method, which is called by the layout system as part of a rendering pass.
		protected override void OnRender(DrawingContext drawingContext)
		{
			var adornedElementRect = new Rect(AdornedElement.DesiredSize);

			// Some arbitrary drawing implements.
			var renderBrush = new SolidColorBrush(Colors.Green)
			{
				Opacity = 0.2
			};
			var renderPen = new Pen(new SolidColorBrush(Colors.Navy), 1.5);

			const double RenderRadius = 5.0;

			// Draw a circle at each corner.
			drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.TopLeft, RenderRadius, RenderRadius);
			drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.TopRight, RenderRadius, RenderRadius);
			drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.BottomLeft, RenderRadius, RenderRadius);
			drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.BottomRight, RenderRadius, RenderRadius);
		}
	}
}