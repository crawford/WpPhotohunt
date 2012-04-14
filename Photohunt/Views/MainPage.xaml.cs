using Microsoft.Phone.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Tasks;
using System.Windows;
using System;
using Photohunt.Models;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using Microsoft.Phone.Shell;

namespace Photohunt.Views
{
    public partial class MainPage : PhoneApplicationPage
    {
        private bool _firstLaunch = true;

        public MainPage()
        {
            InitializeComponent();
            AbbClues = ((ApplicationBarIconButton)ApplicationBar.Buttons[0]);
            AbbPhoto = ((ApplicationBarIconButton)ApplicationBar.Buttons[1]);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (DataContext == null)
            {
                DataContext = App.MainViewModel;
            }

            AbbClues.IsEnabled = App.ContestService.ActiveGame;
            AbbPhoto.IsEnabled = App.ContestService.ActiveGame && App.ContestService.PhotoCount < App.ContestService.MaxPhotoCount;

            if (_firstLaunch)
            {
                _firstLaunch = false;
                if (App.SettingsService.AutostartCamera)
                {
                    AbbPhoto_Click(null, null);
                }
            }
        }

        private void AbbPhoto_Click(object sender, System.EventArgs e)
        {
            App.PhotoInfoViewModel.CurrentPhoto = null;
            NavigationService.Navigate(new Uri("/Views/PhotoInfoPage.xaml", UriKind.Relative));
        }

        private void Image_Click(object sender, System.EventArgs e)
        {
            Image image = sender as Image;
            if (image == null)
                return;

            Photo photo = (Photo)image.DataContext;

            App.PhotoInfoViewModel.CurrentPhoto = photo;
            NavigationService.Navigate(new Uri("/Views/PhotoInfoPage.xaml", UriKind.Relative));
        }

        private void MbbSettings_Click(object sender, System.EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/SettingsPage.xaml", UriKind.Relative));
        }

        private void MbbAbout_Click(object sender, System.EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/AboutPage.xaml", UriKind.Relative));
        }

        private void AbbClues_Click(object sender, System.EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/ClueList.xaml", UriKind.Relative));
        }
    }
}
