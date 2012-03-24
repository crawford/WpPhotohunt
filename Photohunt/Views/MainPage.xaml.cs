using Microsoft.Phone.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Tasks;
using System.Windows;
using System;
using Photohunt.Models;

namespace Photohunt.Views
{
    public partial class MainPage : PhoneApplicationPage
    {
        CameraCaptureTask cameraCaptureTask;
        bool imageCaptured;


        public MainPage()
        {
            InitializeComponent();
            imageCaptured = false;
            cameraCaptureTask = new CameraCaptureTask();
            cameraCaptureTask.Completed += new System.EventHandler<PhotoResult>(cameraCaptureTask_Completed);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (imageCaptured)
            {
                NavigationService.StopLoading();
                NavigationService.Navigate(new Uri("/Views/PhotoInfoPage.xaml", UriKind.Relative));
                imageCaptured = false;
                return;
            }

            base.OnNavigatedTo(e);

            if (DataContext == null)
                DataContext = App.MainViewModel;
        }

        private void AbbPhoto_Click(object sender, System.EventArgs e)
        {
            try
            {
                cameraCaptureTask.Show();
            }
            catch (System.InvalidOperationException)
            {
                MessageBox.Show("An error occurred.");
            }
        }

        void cameraCaptureTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                App.ContestService.CurrentPhoto = App.ContestService.CreatePhoto(new Uri(e.OriginalFileName));
                imageCaptured = true;
            }
        }
    }
}
