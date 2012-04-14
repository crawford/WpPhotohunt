using Microsoft.Phone.Controls;
using System.Windows.Navigation;
using System;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using Microsoft.Phone.Tasks;
using System.Windows;
using System.IO.IsolatedStorage;
using System.IO;
using Microsoft.Phone.Shell;

namespace Photohunt.Views
{
    public partial class PhotoInfoPage : PhoneApplicationPage
    {
        CameraCaptureTask cameraCaptureTask;
        bool cameraCancelled = false;

        public PhotoInfoPage()
        {
            InitializeComponent();
            AbbSave = ((ApplicationBarIconButton)ApplicationBar.Buttons[0]);
            cameraCaptureTask = new CameraCaptureTask();
            cameraCaptureTask.Completed += new System.EventHandler<PhotoResult>(cameraCaptureTask_Completed);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (DataContext == null)
                DataContext = App.PhotoInfoViewModel;

            ChkJudge.IsEnabled = ChkJudge.IsChecked.GetValueOrDefault(false) || (App.ContestService.JudgedPhotoCount < App.ContestService.MaxJudgedPhotoCount);
            AbbSave.IsEnabled = App.ContestService.ActiveGame;

            if (App.PhotoInfoViewModel.CurrentPhoto == null)
            {
                if (cameraCancelled)
                {
                    cameraCancelled = false;
                    NavigationService.GoBack();
                }
                else
                {
                    try
                    {
                        cameraCaptureTask.Show();
                    }
                    catch (System.InvalidOperationException)
                    {
                        MessageBox.Show("Error loading camera");
                    }
                }
            }
            else
            {
                BitmapImage image = new BitmapImage();
                try
                {
                    lock (App.IsolatedStorageFileLock)
                    {
                        using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
                        {
                            using (IsolatedStorageFileStream stream = store.OpenFile(App.PhotoInfoViewModel.CurrentPhoto.Path.AbsolutePath, FileMode.Open))
                            {
                                image.SetSource(stream);
                            }
                        }
                    }
                }
                catch (IsolatedStorageException)
                {
                    MessageBox.Show("Error loading image from storage");
                }
                ImgPhoto.Source = image;
            }
        }

        private void AbbSave_Click(object sender, System.EventArgs e)
        {
            // Bug workaround http://forums.create.msdn.com/forums/p/84887/517667.aspx
            App.PhotoInfoViewModel.CurrentPhoto.Notes = TxtNotes.Text;
            //TxtNotes.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            ChkJudge.GetBindingExpression(CheckBox.IsCheckedProperty).UpdateSource();

            App.MainViewModel.UpdatePointCount();

            NavigationService.GoBack();
        }

        private void AbbCancel_Click(object sender, System.EventArgs e)
        {
            NavigationService.GoBack();
        }

        void cameraCaptureTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                lock (App.IsolatedStorageFileLock)
                {
                    using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        if (!store.DirectoryExists(App.PHOTO_DIRECTORY))
                            store.CreateDirectory(App.PHOTO_DIRECTORY);

                        string fileName = Path.Combine(App.PHOTO_DIRECTORY, Guid.NewGuid().ToString());
                        using (IsolatedStorageFileStream stream = store.CreateFile(fileName))
                        {
                            byte[] buffer = new byte[4096];
                            int bytesRead;
                            while ((bytesRead = e.ChosenPhoto.Read(buffer, 0, buffer.Length)) != 0)
                                stream.Write(buffer, 0, bytesRead);
                        }

                        App.PhotoInfoViewModel.CurrentPhoto = App.ContestService.CreatePhoto(new Uri(fileName, UriKind.Absolute));
                    }
                }
            }
            else
            {
                cameraCancelled = true;
            }
        }

        private void BtnClues_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/CluePicker.xaml", UriKind.Relative));
        }
    }
}