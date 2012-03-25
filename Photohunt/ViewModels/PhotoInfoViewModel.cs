using System.ComponentModel;
using Photohunt.Models;
using System.Collections.ObjectModel;
using Photohunt.Data;
using System.IO;

namespace Photohunt.ViewModels
{
    public class PhotoInfoViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        Photo _currentPhoto;

        public PhotoInfoViewModel()
        {
            _currentPhoto = null;
        }

        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #region Getters and Setters

        public Photo CurrentPhoto
        {
            get
            {
                return _currentPhoto;
            }
            set
            {
                if (_currentPhoto != value)
                {
                    _currentPhoto = value;
                    NotifyPropertyChanged("CurrentPhoto");
                }
            }
        }

        #endregion
    }
}
