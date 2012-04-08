using System.Windows;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Navigation;
using Photohunt.ViewModels;
using Photohunt.Data;

namespace Photohunt
{
    public partial class App : Application
    {

        // Easy access to the root frame
        public PhoneApplicationFrame RootFrame { get; private set; }

        public static object IsolatedStorageFileLock = new object();

        #region ViewModels

        private static MainViewModel _mainViewModel = null;
        private static PhotoInfoViewModel _photoInfoViewModel = null;
        private static CluesViewModel _cluesViewModel = null;

        // Easy access to the viewmodels
        public static MainViewModel MainViewModel
        {
            get
            {
                if (_mainViewModel == null)
                    _mainViewModel = new MainViewModel();

                return _mainViewModel;
            }
        }

        public static PhotoInfoViewModel PhotoInfoViewModel
        {
            get
            {
                if (_photoInfoViewModel == null)
                    _photoInfoViewModel = new PhotoInfoViewModel();

                return _photoInfoViewModel;
            }
        }

        public static CluesViewModel CluesViewModel
        {
            get
            {
                if (_cluesViewModel == null)
                    _cluesViewModel = new CluesViewModel();

                return _cluesViewModel;
            }
        }

        #endregion

        #region Services

        private static ContestService _contestService = null;
        private static SettingsService _settingsService = null;
        private static ApiService _apiService = null;

        public static ContestService ContestService
        {
            get
            {
                if (_contestService == null)
                    _contestService = new ContestService();

                return _contestService;
            }
        }

        public static SettingsService SettingsService
        {
            get
            {
                if (_settingsService == null)
                    _settingsService = new SettingsService();

                return _settingsService;
            }
        }

        public static ApiService ApiService
        {
            get
            {
                if (_apiService == null)
                    _apiService = new ApiService();

                return _apiService;
            }
        }

        #endregion

        public const string PHOTO_DIRECTORY = "/photos";

        // Constructor
        public App()
        {
            // Global handler for uncaught exceptions. 
            // Note that exceptions thrown by ApplicationBarItem.Click will not get caught here.
            UnhandledException += Application_UnhandledException;

            // Standard Silverlight initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();
        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            ContestService.Load();
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            ContestService.Load();
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
        }

        // Code to execute if a navigation fails
        void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion
    }
}
