using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfPainter.Extensions;
using WpfPainter.ViewModel;

namespace WpfPainter.Controls
{
	/// <summary>
	/// Interaction logic for MultiCanvas.xaml
	/// </summary>
	public partial class MultiCanvas : IApplicationAction
	{
		public static readonly DependencyProperty SelectedItemProperty =
			DependencyProperty.Register(
				"SelectedItem",
				typeof (object),
				typeof (MultiCanvas),
				new PropertyMetadata(default(object)));

		public static readonly DependencyProperty ActiveBorderBrushProperty =
			DependencyProperty.Register(
				"ActiveBorderBrush",
				typeof (Brush),
				typeof (DrawingCanvas),
				new PropertyMetadata(default(Brush)));

		public static readonly DependencyProperty InactiveBorderBrushProperty =
			DependencyProperty.Register(
				"InactiveBorderBrush",
				typeof (Brush),
				typeof (DrawingCanvas),
				new PropertyMetadata(default(Brush)));

		public MultiCanvas()
		{
			InitializeComponent();

			// Вариант рефакторинга:
			// Вся логика находится непосредственно в слое DrawingCanvas.
			// Чтобы перенести логику на уровень приложения, то есть сюда,
			// необходимо свойство, которое возвращает текущий слой DrawingCanvas.
		}

		public Brush ActiveBorderBrush
		{
			get { return (Brush) GetValue(ActiveBorderBrushProperty); }
			set { SetValue(ActiveBorderBrushProperty, value); }
		}

		public Brush InactiveBorderBrush
		{
			get { return (Brush) GetValue(InactiveBorderBrushProperty); }
			set { SetValue(InactiveBorderBrushProperty, value); }
		}

		public object SelectedItem
		{
			get { return GetValue(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
		}

		public void Save()
		{
			var enc = new PngBitmapEncoder();

			var canvas = _itemsControl.FindVisualChild<Canvas>();
			enc.Frames.Add(BitmapFrame.Create(CreateBitmap(canvas)));

			//for (var i = 0; i < _itemsControl.Items.Count; i++)
			//{
			//	var content = _itemsControl
			//		.ItemContainerGenerator
			//		.ContainerFromIndex(i)
			//		.Cast<ContentPresenter>();

			//	var canvas = content.FindVisualChild<Canvas>();
			//	enc.Frames.Add(BitmapFrame.Create(CreateBitmap(canvas)));
			//}

			using (var stream = File.Create("picture.png"))
			{
				enc.Save(stream);
			}
		}

		private RenderTargetBitmap CreateBitmap(UIElement canvas)
		{
			var rtb = new RenderTargetBitmap(
				(int) canvas.RenderSize.Width,
				(int) canvas.RenderSize.Height,
				96d,
				96d,
				PixelFormats.Default);

			rtb.Render(canvas);
			return rtb;
		}
	}
}