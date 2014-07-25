using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using GalaSoft.MvvmLight;

namespace WpfPainter.ViewModel
{
	public class ToolsViewModel : ViewModelBase
	{
		public ToolsViewModel()
		{
			Brush = Brushes.Blue;
			Color = Colors.Blue;
			Pen = 1;
		}

		public Brush Brush
		{
			get { return _brush; }
			set
			{
				_brush = value;
				RaisePropertyChanged("Brush");
			}
		}

		public Color Color
		{
			get { return _color; }
			set
			{
				_color = value;
				RaisePropertyChanged("Color");
				Brush = new SolidColorBrush(value);
			}
		}

		public int Pen
		{
			get { return (int) _pen; }
			set
			{
				_pen = value;
				RaisePropertyChanged("Pen");
			}
		}

		public IEnumerable<int> Pens
		{
			get { return Enumerable.Range(1, 10).ToArray(); }
		}

		private Brush _brush;
		private Color _color;
		private double _pen;
	}
}