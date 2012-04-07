using System;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;

namespace Photohunt
{
    public partial class Clues : PhoneApplicationPage
    {
        public Clues()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (DataContext == null)
                DataContext = App.CluesViewModel;
        }
    }
}
