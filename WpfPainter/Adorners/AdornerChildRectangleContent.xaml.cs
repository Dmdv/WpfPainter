using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfPainter.Adorners
{
	/// <summary>
	/// Interaction logic for AdornerChildRectangleContent.xaml
	/// </summary>
	public partial class AdornerChildRectangleContent : UserControl
	{
		public AdornerChildRectangleContent()
		{
			InitializeComponent();
		}

		protected override void OnRender(DrawingContext drawingContext)
		{
			var rect = new Rect(new Point(0, 0), new Size(ActualWidth, ActualHeight));
			drawingContext.DrawRectangle(Brushes.Chocolate, null, rect);
			base.OnRender(drawingContext);
		}
	}
}