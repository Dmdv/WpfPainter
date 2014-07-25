using Common.Contracts;
using GalaSoft.MvvmLight;
using WpfPainter.Commands;
using WpfPainter.Model;

namespace WpfPainter.ViewModel
{
	public class UserActionsViewModel : ViewModelBase
	{
		public UserActionsViewModel(LayersViewModel layersViewModel)
		{
			_layersViewModel = Guard.GetNotNull(layersViewModel, "layersViewModel");

			CreateNewLayerCommand = new DelegateCommand<object>(CreateNewLayer, CreateNewLayerCanExecute);
			DeleteCommand = new DelegateCommand<object>(Delete, DeleteCanExecute);
			SaveCommand = new DelegateCommand<IApplicationAction>(Save, SaveCanExecute);

			IsDrawing = true;
		}

		public DelegateCommand<object> CreateNewLayerCommand { get; private set; }

		public UserActions CurrentTool
		{
			get { return _currentTool; }
			set
			{
				_currentTool = value;
				RaisePropertyChanged("CurrentTool");
			}
		}

		public DelegateCommand<object> DeleteCommand { get; set; }

		public bool IsDrawing
		{
			get { return _isDrawing; }
			set
			{
				_isDrawing = value;
				if (value)
				{
					CurrentTool = UserActions.Drawing;
				}
				RaisePropertyChanged("IsDrawing");
			}
		}

		public bool IsSelectMove
		{
			get { return _isSelectMove; }
			set
			{
				_isSelectMove = value;
				if (value)
				{
					CurrentTool = UserActions.SelectMove;
				}
				RaisePropertyChanged("IsSelectMove");
			}
		}

		public bool IsSelectMultiple
		{
			get { return _isSelectMultiple; }
			set
			{
				_isSelectMultiple = value;
				if (value)
				{
					CurrentTool = UserActions.SelectMultiple;
				}
				RaisePropertyChanged("IsSelectMultiple");
			}
		}

		public DelegateCommand<IApplicationAction> SaveCommand { get; set; }

		private void CreateNewLayer(object obj)
		{
			_layersViewModel.AddNew();
		}

		private bool CreateNewLayerCanExecute(object arg)
		{
			return true;
		}

		private void Delete(object obj)
		{
			_layersViewModel.DeleteSelected();
		}

		private bool DeleteCanExecute(object arg)
		{
			return true;
		}

		private void Save(IApplicationAction obj)
		{
			obj.Save();
		}

		private bool SaveCanExecute(IApplicationAction arg)
		{
			return true;
		}

		private readonly LayersViewModel _layersViewModel;
		private UserActions _currentTool;
		private bool _isDrawing;
		private bool _isSelectMove;
		private bool _isSelectMultiple;
	}
}