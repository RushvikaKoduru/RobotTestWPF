/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:TestApp"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Interview1;
using Simulator;
using TestApp.ViewModels;
using TestApp.ViewModels.Interfaces;

namespace TestApp
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<IStudio, Studio>();
            SimpleIoc.Default.Register<StudioViewModel>();
            SimpleIoc.Default.Register<IRobotMoveModelFactory, RobotMoveModelFactory>();
            SimpleIoc.Default.Register<IRobotViewModelFactory, RobotViewModelFactory>();
        }

        /// <summary>
        /// The view model for the app's main window
        /// </summary>
        public StudioViewModel Studio
        {
            get
            {
                return ServiceLocator.Current.GetInstance<StudioViewModel>();
            }
        }

        /// <summary>
        /// Mvvm light shut down clean up
        /// </summary>
        public static void Cleanup()
        {
            ServiceLocator.Current.GetInstance<StudioViewModel>().Cleanup();
            SimpleIoc.Default.Unregister<StudioViewModel>();

            Messenger.Reset();
        }
    }
}