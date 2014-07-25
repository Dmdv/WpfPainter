using System.Globalization;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Common.Extensions;
using WpfPainter.Adorners;
using WpfPainter.Model;
using WpfPainter.Primitives;

namespace WpfPainter.Controls
{
	public class DrawingCanvas : VisualCanvas
	{
		public static readonly DependencyProperty SignatureProperty =
			DependencyProperty.Register(
				"Signature",
				typeof (string),
				typeof (DrawingCanvas),
				new PropertyMetadata("Layer"));

		public static readonly DependencyProperty UserActionProperty =
			DependencyProperty.Register(
				"UserAction",
				typeof (UserActions),
				typeof (DrawingCanvas),
				new PropertyMetadata(default(UserActions)));

		public static readonly DependencyProperty ColorProperty =
			DependencyProperty.Register(
				"Color",
				typeof (Color),
				typeof (DrawingCanvas),
				new PropertyMetadata(default(Color)));

		public static readonly DependencyProperty PenWidthProperty =
			DependencyProperty.Register(
				"PenWidth",
				typeof (double),
				typeof (DrawingCanvas),
				new PropertyMetadata(1.0));

		public static readonly DependencyProperty IsSelectedProperty =
			DependencyProperty.Register(
				"IsSelected",
				typeof (bool),
				typeof (DrawingCanvas),
				new PropertyMetadata(default(bool)));

		public static readonly DependencyProperty SelectedSelfProperty =
			DependencyProperty.Register(
				"SelectedSelf",
				typeof (object),
				typeof (DrawingCanvas),
				new PropertyMetadata(default(object)));

		public DrawingCanvas()
		{
			Loaded += OnLoaded;
		}

		public Color Color
		{
			get { return (Color) GetValue(ColorProperty); }
			set { SetValue(ColorProperty, value); }
		}

		public bool IsSelected
		{
			get { return (bool) GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}

		public double PenWidth
		{
			get { return (double) GetValue(PenWidthProperty); }
			set { SetValue(PenWidthProperty, value); }
		}

		public object SelectedSelf
		{
			get { return GetValue(SelectedSelfProperty); }
			set { SetValue(SelectedSelfProperty, value); }
		}

		public string Signature
		{
			get { return (string) GetValue(SignatureProperty); }
			set { SetValue(SignatureProperty, value); }
		}

		public UserActions UserAction
		{
			get { return (UserActions) GetValue(UserActionProperty); }
			set { SetValue(UserActionProperty, value); }
		}

		protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
		{
			return new PointHitTestResult(this, hitTestParameters.HitPoint);
		}

		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			base.OnMouseDown(e);

			if (IsSelectionMode(e))
			{
				IsSelected = true;
				SelectedSelf = this;
				_adorner.Visibility = Visibility.Visible;
			}

			if (IsDrawingMode(e))
			{
				OnMouseDownDrawing(e);
			}
			else if (IsMultiSelectMode(e))
			{
				_selectionSquare = new DrawingObject();

				AddVisual(_selectionSquare);

				_selectionSquareTopLeft = e.GetPosition(this);

				CaptureMouse();
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			if (IsDrawingMode(e))
			{
				OnMouseMoveDrawing(e);
				CaptureMouse();
			}
			else if (IsMultiSelectMode(e))
			{
				var pointDragged = e.GetPosition(this);
				DrawSelectionSquare(_selectionSquareTopLeft, pointDragged);
			}
		}

		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			base.OnMouseUp(e);

			if (UserAction == UserActions.SelectMultiple)
			{
				var geometry = new RectangleGeometry(new Rect(_selectionSquareTopLeft, e.GetPosition(this)));
				var visualsInRegion = GetVisuals(geometry);

				if (MessageBox.Show(
					"Удалить выделенные {0} объекта? ".FormatString(visualsInRegion.Length),
					"Удаление объектов",
					MessageBoxButton.YesNo,
					MessageBoxImage.Question) == MessageBoxResult.Yes)
				{
					foreach (var drawingVisual in visualsInRegion)
					{
						// TODO: Необходимо доработка для выделения выбранных объектов.
						// Временное решение - удалять.
						drawingVisual.IsSelected = true;
						DeleteVisual(drawingVisual);
					}
				}

				DeleteVisual(_selectionSquare);
			}

			IsSelected = false;
			_adorner.Visibility = Visibility.Hidden;
			ReleaseMouseCapture();
		}

		protected override void OnRender(DrawingContext drawingContext)
		{
			base.OnRender(drawingContext);
		}

		private void AddAdorner()
		{
			var adornerLayer = AdornerLayer.GetAdornerLayer(this);
			_adorner = new ResizingAdorner(this)
			{
				Visibility = Visibility.Hidden
			};
			adornerLayer.Add(_adorner);
		}

		private void AddSignature()
		{
			var visual = new DrawingObject();

			using (var dc = visual.RenderOpen())
			{
				dc.DrawText(
					new FormattedText(
						Signature,
						CultureInfo.InvariantCulture,
						FlowDirection.LeftToRight,
						new Typeface(
							new FontFamily("Arial"),
							FontStyles.Italic,
							FontWeights.Regular,
							FontStretches.Normal),
						24.0,
						Brushes.Gray),
					new Point(10, 10));
			}

			AddVisual(visual);
		}

		private void DrawCircle(Point point, double radius)
		{
			var visual = new DrawingObject();
			var brush = new SolidColorBrush(Color);

			using (var dc = visual.RenderOpen())
			{
				dc.DrawEllipse(brush, new Pen(brush, 1), point, radius, radius);
			}

			AddVisual(visual);
		}

		private void DrawSelectionSquare(Point point1, Point point2)
		{
			_selectionSquarePen.DashStyle = DashStyles.Dash;

			using (var dc = _selectionSquare.RenderOpen())
			{
				dc.DrawRectangle(_selectionSquareBrush,
					_selectionSquarePen,
					new Rect(point1, point2));
			}
		}

		private void DrawWithContext(MouseEventArgs e)
		{
			var visual = new DrawingObject();

			using (var dc = visual.RenderOpen())
			{
				var p1 = new Point(_currentPosition.X, _currentPosition.Y);
				var p2 = new Point(e.GetPosition(this).X, e.GetPosition(this).Y);

				Brush brush = new SolidColorBrush(Color);

				if (PenWidth > 2)
				{
					DrawCircle(p2, PenWidth/2);
				}
				else
				{
					dc.DrawLine(new Pen(brush, PenWidth), p1, p2);
				}
			}

			AddVisual(visual);
		}

		private bool IsDrawingMode(MouseEventArgs e)
		{
			return UserAction == UserActions.Drawing && e.LeftButton == MouseButtonState.Pressed;
		}

		private bool IsMultiSelectMode(MouseEventArgs e)
		{
			return UserAction == UserActions.SelectMultiple && e.LeftButton == MouseButtonState.Pressed;
		}

		private bool IsSelectionMode(MouseEventArgs e)
		{
			return e.LeftButton == MouseButtonState.Pressed;
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			AddSignature();
			AddAdorner();
		}

		private void OnMouseDownDrawing(MouseEventArgs e)
		{
			UpdateCurrentPosition(e);
		}

		private void OnMouseMoveDrawing(MouseEventArgs e)
		{
			DrawWithContext(e);

			UpdateCurrentPosition(e);
		}

		private void UpdateCurrentPosition(MouseEventArgs e)
		{
			_currentPosition = e.GetPosition(this);
		}

		private readonly Brush _selectionSquareBrush = Brushes.Transparent;
		private readonly Pen _selectionSquarePen = new Pen(Brushes.Black, 2);
		private Adorner _adorner;
		private Point _currentPosition;
		private DrawingObject _selectionSquare;
		private Point _selectionSquareTopLeft;
	}
}