using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using System;

namespace Photohunt
{
    public partial class AboutPage : PhoneApplicationPage
    {
        public AboutPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            TxtMemory.Text = string.Format("{0} KiB currently used", (long)Microsoft.Phone.Info.DeviceExtendedProperties.GetValue("ApplicationCurrentMemoryUsage") / 1024);
            TxtMemoryPeak.Text = string.Format("{0} KiB peak usage", (long)Microsoft.Phone.Info.DeviceExtendedProperties.GetValue("ApplicationPeakMemoryUsage") / 1024);
        }

        private void Report_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	WebBrowserTask task = new WebBrowserTask();
            task.Uri = new Uri(@"https://github.com/crawford/WpPhotohunt/issues");
            task.Show();
        }
    }
}
