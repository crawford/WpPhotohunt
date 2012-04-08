using System.Windows.Data;
using System.Globalization;
using System;
using System.Windows.Media.Imaging;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;

namespace Photohunt.Converters
{
    public class ImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Uri uri = (Uri)value;
            BitmapImage image = new BitmapImage();

            try
            {
                lock (App.IsolatedStorageFileLock)
                {
                    using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        using (IsolatedStorageFileStream stream = store.OpenFile(uri.AbsolutePath, FileMode.Open, FileAccess.Read))
                        {
                            image.SetSource(stream);
                        }

                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Error loading image from storage");
            }
            
            return image;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
