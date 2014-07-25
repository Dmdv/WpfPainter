using GalaSoft.MvvmLight;

namespace WpfPainter.ViewModel
{
	/// <summary>
	/// This class contains properties that the main View can data bind to.
	/// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
	/// </summary>
	public class MainViewModel : ViewModelBase
	{
		/// <summary>
		/// Initializes a new instance of the MainViewModel class.
		/// </summary>
		public MainViewModel()
		{
			if (IsInDesignMode)
			{
				// Code runs in Blend --> create design time data.
			}
		}
	}
}