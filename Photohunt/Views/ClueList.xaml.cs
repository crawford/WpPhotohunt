using System;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;

namespace Photohunt
{
    public partial class ClueList : PhoneApplicationPage
    {
        public ClueList()
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
