using Microsoft.Phone.Controls;
using System.Windows.Navigation;
using System;
using System.Windows.Media.Imaging;
using System.Windows.Controls;

namespace Photohunt.Views
{
    public partial class PhotoInfoPage : PhoneApplicationPage
    {
        public PhotoInfoPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (DataContext == null)
                DataContext = App.ContestService.CurrentPhoto;
        }

        private void AbbSave_Click(object sender, System.EventArgs e)
        {
            TxtNotes.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            ChkJudge.GetBindingExpression(CheckBox.IsCheckedProperty).UpdateSource();
            NavigationService.GoBack();
        }

        private void AbbCancel_Click(object sender, System.EventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}