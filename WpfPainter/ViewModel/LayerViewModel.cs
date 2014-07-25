using System.Windows.Media;
using Common.Contracts;
using GalaSoft.MvvmLight;
using WpfPainter.Model;

namespace WpfPainter.ViewModel
{
	public class LayerViewModel : ViewModelBase
	{
		public LayerViewModel(Layer layer, LayersViewModel layersViewModel)
		{
			_layersViewModel = layersViewModel;
			_layer = Guard.GetNotNull(layer, "layer");
			BorderBrush = Brushes.SteelBlue;
		}

		public Brush BorderBrush
		{
			get { return _borderBrush; }
			set
			{
				_borderBrush = value;
				RaisePropertyChanged("BorderBrush");
			}
		}

		public bool IsSelected
		{
			get { return _isSelected; }
			set
			{
				_isSelected = value;
				if (_isSelected)
				{
					_layersViewModel.SelectedItem = this;
				}
				RaisePropertyChanged("IsSelected");
			}
		}

		public bool IsVisible
		{
			get { return _layer.IsVisible; }
			set
			{
				_layer.IsVisible = value;
				RaisePropertyChanged("IsVisible");
			}
		}

		public string Name
		{
			get { return _layer.Name; }
			set
			{
				_layer.Name = value;
				RaisePropertyChanged("Name");
			}
		}

		public int X
		{
			get { return _layer.X; }
			set
			{
				_layer.X = value;
				RaisePropertyChanged("X");
			}
		}

		public int Y
		{
			get { return _layer.Y; }
			set
			{
				_layer.Y = value;
				RaisePropertyChanged("Y");
			}
		}

		public int ZIndex
		{
			get { return _layer.ZIndex; }
			set
			{
				_layer.ZIndex = value;
				RaisePropertyChanged("ZIndex");
			}
		}

		private readonly Layer _layer;
		private readonly LayersViewModel _layersViewModel;
		private Brush _borderBrush;
		private bool _isSelected;
	}
}