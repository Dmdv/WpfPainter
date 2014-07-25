using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace WpfPainter.ViewModel
{
	/// <summary>
	/// This class contains static references to all the view models in the
	/// application and provides an entry point for the bindings.
	/// In the View: DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
	/// </summary>
	public class ViewModelLocator
	{
		/// <summary>
		/// Initializes a new instance of the ViewModelLocator class.
		/// </summary>
		public ViewModelLocator()
		{
			ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

			//if (ViewModelBase.IsInDesignModeStatic)
			//{
			//    // Create design time view services and models
			//    SimpleIoc.Default.Register<IDataService, DesignDataService>();
			//}
			//else
			//{
			//    // Create run time view services and models
			//    SimpleIoc.Default.Register<IDataService, DataService>();
			//}

			SimpleIoc.Default.Register<MainViewModel>();
			SimpleIoc.Default.Register<LayersViewModel>();
			SimpleIoc.Default.Register<UserActionsViewModel>();
			SimpleIoc.Default.Register<ToolsViewModel>();
		}

		public LayersViewModel LayersViewModel
		{
			get { return ServiceLocator.Current.GetInstance<LayersViewModel>(); }
		}

		public ToolsViewModel Tools
		{
			get { return ServiceLocator.Current.GetInstance<ToolsViewModel>(); }
		}

		public UserActionsViewModel UserActions
		{
			get { return ServiceLocator.Current.GetInstance<UserActionsViewModel>(); }
		}

		public static void Cleanup()
		{
			// TODO Clear the ViewModels
		}
	}
}