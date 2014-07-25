using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using Common.Extensions;
using GalaSoft.MvvmLight;
using GongSolutions.Wpf.DragDrop;
using WpfPainter.Model;

namespace WpfPainter.ViewModel
{
	public class LayersViewModel : ViewModelBase, IDropTarget
	{
		public LayersViewModel()
		{
			Layers = new ObservableCollection<LayerViewModel>();

			AddNew();
			AddNew();

			ActiveBorderBrush = Brushes.Tomato;
			InactiveBorderBrush = Brushes.SteelBlue;
		}

		public Brush ActiveBorderBrush
		{
			get { return _activeBorderBrush; }
			set
			{
				_activeBorderBrush = value;
				RaisePropertyChanged("ActiveBorderBrush");
			}
		}

		public Brush InactiveBorderBrush
		{
			get { return _inactiveBorderBrush; }
			set
			{
				_inactiveBorderBrush = value;
				RaisePropertyChanged("InactiveBorderBrush");
			}
		}

		public ObservableCollection<LayerViewModel> Layers { get; set; }

		public LayerViewModel SelectedItem
		{
			get { return _selectedItem; }
			set
			{
				_selectedItem = value;

				foreach (var layerViewModel in Layers)
				{
					layerViewModel.BorderBrush = InactiveBorderBrush;
				}

				if (_selectedItem != null)
				{
					_selectedItem.BorderBrush = ActiveBorderBrush;
				}

				RaisePropertyChanged("SelectedItem");
			}
		}

		public void DragOver(IDropInfo dropInfo)
		{
			var sourceItem = dropInfo.Data.As<LayerViewModel>();
			var targetItem = dropInfo.TargetItem.As<LayerViewModel>();

			if (sourceItem != null && targetItem != null)
			{
				dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
				dropInfo.Effects = DragDropEffects.Move;
			}
		}

		public void Drop(IDropInfo dropInfo)
		{
			var sourceItem = dropInfo.DragInfo.SourceItem.Cast<LayerViewModel>();
			var targetItem = dropInfo.TargetItem.Cast<LayerViewModel>();

			var targetItemZIndex = targetItem.ZIndex;
			var sourceItemZIndex = sourceItem.ZIndex;

			var targetItemIndex = Layers.IndexOf(targetItem);

			Layers.Move(dropInfo.DragInfo.SourceIndex, targetItemIndex);

			sourceItem.ZIndex = targetItemZIndex;
			targetItem.ZIndex = sourceItemZIndex;
		}

		public void AddNew()
		{
			var count = Layers.Count;

			Layers.Add(
				new LayerViewModel(
					new Layer
					{
						ZIndex = - count*100,
						IsVisible = true,
						Name = "Layer {0}".FormatString(++count)
					},
					this));
		}

		public void DeleteSelected()
		{
			if (SelectedItem != null)
			{
				if (MessageBox.Show(
					"Удалить выбранный слой '{0}' ?".FormatString(SelectedItem.Name),
					"Удалить слой",
					MessageBoxButton.YesNo,
					MessageBoxImage.Question) == MessageBoxResult.Yes)
				{
					Layers.Remove(SelectedItem);
				}
			}
		}

		private Brush _activeBorderBrush;
		private Brush _inactiveBorderBrush;
		private LayerViewModel _selectedItem;
	}
}