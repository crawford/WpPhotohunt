using System;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using System.Windows.Controls;
using System.Collections.Generic;
using Photohunt.Models;
using System.Windows.Media;

namespace Photohunt
{
    public partial class CluePicker : PhoneApplicationPage
    {
        public CluePicker()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (DataContext == null)
                DataContext = App.CluesViewModel;

            App.CluesViewModel.SelectClues(App.PhotoInfoViewModel.CurrentPhoto.Clues);
        }

        private void AbbApply_Click(object sender, System.EventArgs e)
        {
            App.PhotoInfoViewModel.CurrentPhoto.Clues = App.CluesViewModel.GetClues();
            NavigationService.GoBack();
        }

        private void AbbCancel_Click(object sender, System.EventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
