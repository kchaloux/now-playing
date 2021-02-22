using System;
using System.Reflection;
using System.Windows;
using NowPlaying.ViewModels;
using NowPlaying.Views;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Unity;

namespace NowPlaying
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    internal partial class App : PrismApplication
    {
        /// <summary>
        /// Used to register types with the container that will be used by your application.
        /// </summary>
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<ConfigurationDialog, ConfigurationDialogViewModel>();
        }

        /// <summary>Creates the shell or main window of the application.</summary>
        /// <returns>The shell of the application.</returns>
        protected override Window CreateShell()
        {
            var mainWindow = Container.Resolve<MainWindow>();
            return mainWindow;
        }
    }
}
